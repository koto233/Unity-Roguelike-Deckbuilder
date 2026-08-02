using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitFramework;
using LitFramework.Config;
using UnityEngine;

public class MapService
{
    public Dictionary<string, MapNodeData> CurrentMap { get; private set; }
    public IReadOnlyCollection<MapNodeData> CurrentMapList => CurrentMap?.Values;
    private IConfigService _configService;
    private MapNodeData _currentNode;

    private IConfigService ConfigService =>
    _configService ??= ServiceLocator.Get<IConfigService>();
    public MapNodeData GetNode(string id) => CurrentMap?.GetValueOrDefault(id);

    public List<MapNodeData> GetNodesAtRow(int row) => CurrentMap?.Values.Where(n => n.Row == row).ToList();
    public void GenerateMap(int templateId)
    {
        var allConfigs = ConfigService.GetTable<MapConfig>().GetAll();
        if (allConfigs == null) return;
        var rowConfigs = allConfigs
      .Where(c => c.Templateld == templateId)
      .OrderBy(c => c.Row)
      .ToList();
        CurrentMap = MapGenerator.Generate(rowConfigs);
        InitStartNode();
        RefreshInteractableStates();
    }

    private void InitStartNode()
    {
        _currentNode = GetNode("0_0");
        // 访问后自动解锁下一层相邻节点
        foreach (var nextId in _currentNode.NextNodes)
        {
            var next = GetNode(nextId);
            if (next != null && !next.IsVisited)
                next.IsLocked = false;
        }
    }
    public void VisitNode(string id)
    {
        var node = GetNode(id);
        if (node != null && !node.IsLocked && !node.IsVisited && CanSelectNode(id))
        {
            node.IsVisited = true;
            _currentNode = node;

            // 访问后自动解锁下一层相邻节点
            foreach (var nextId in node.NextNodes)
            {
                var next = GetNode(nextId);
                if (next != null && !next.IsVisited)
                    next.IsLocked = false;
            }
        }
        RefreshInteractableStates();
    }

    public bool CanSelectNode(string id)
    {
        var node = GetNode(id);
        if (node == null) return false;
        if (node.IsLocked || node.IsVisited) return false;

        if (_currentNode == null) return false; // 安全兜底

        if (node.Row != _currentNode.Row + 1) return false;
        return _currentNode.NextNodes.Contains(id);
    }
    public void RefreshInteractableStates()
    {
        foreach (var node in CurrentMapList)
        {
            node.IsInteractable = CanSelectNode(node.Id);
        }
    }
    public void ResetMap()
    {
        CurrentMap?.Clear();
    }
}