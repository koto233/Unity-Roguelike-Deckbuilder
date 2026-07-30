using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitFramework;
using LitFramework.Config;
using UnityEngine;

public class MapService
{

    private List<MapNodeData> _mapNodes;

    private IConfigService _configService;
    private string _currentNodeId;

    private IConfigService ConfigService =>
    _configService ??= ServiceLocator.Get<IConfigService>();

    public void GenerateMap(int templateId)
    {
        var allConfigs = ConfigService.GetTable<MapConfig>().GetAll();
        if (allConfigs == null) return;
        var rowConfigs = allConfigs
      .Where(c => c.Templateld == templateId)
      .OrderBy(c => c.Row)
      .ToList();

        _mapNodes = MapGenerator.Generate(rowConfigs);
        RefreshInteractableStates();
    }

    public List<MapNodeData> CurrentMap => _mapNodes;

    public MapNodeData GetNode(string id) => _mapNodes?.Find(n => n.Id == id);

    public List<MapNodeData> GetNodesAtRow(int row) => _mapNodes?.FindAll(n => n.Row == row);


    public void VisitNode(string id)
    {
        var node = GetNode(id);
        if (node != null && !node.IsLocked && !node.IsVisited && CanSelectNode(id))
        {
            node.IsVisited = true;
            _currentNodeId = id;
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

        // 初始状态：只能选起始节点
        if (string.IsNullOrEmpty(_currentNodeId))
            return node.IsStart;

        // 正常状态：必须是当前节点的下一层且在当前节点的 NextNodes 中
        var cur = GetNode(_currentNodeId);
        if (cur == null) return false;
        if (node.Row != cur.Row + 1) return false;
        return cur.NextNodes.Contains(id);
    }
    public void RefreshInteractableStates()
    {
        foreach (var node in _mapNodes)
        {
            node.IsInteractable = CanSelectNode(node.Id);
        }
    }
    public void ResetMap()
    {
        _mapNodes?.Clear();
    }
}