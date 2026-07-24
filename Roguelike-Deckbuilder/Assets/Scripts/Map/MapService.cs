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
    }

    public List<MapNodeData> CurrentMap => _mapNodes;

    public MapNodeData GetNode(string id) => _mapNodes?.Find(n => n.Id == id);

    public List<MapNodeData> GetNodesAtRow(int row) => _mapNodes?.FindAll(n => n.Row == row);

    public void UnlockNode(string id)
    {
        var node = GetNode(id);
        if (node != null && !node.IsVisited && CanSelectNode(id))
        {
            node.IsLocked = false;
        }
    }

    public void VisitNode(string id)
    {
        var node = GetNode(id);
        if (node != null && !node.IsLocked)
        {
            node.IsVisited = true;
            // 访问后自动解锁下一层相邻节点
            foreach (var nextId in node.NextNodes)
            {
                var next = GetNode(nextId);
                if (next != null && !next.IsVisited)
                    next.IsLocked = false;
            }
        }
    }

    public bool CanSelectNode(string id)
    {
        var node = GetNode(id);
        if (node == null) return false;
        // 如果是起始节点，直接可点
        if (node.IsStart) return true;
        // 检查是否有上一行的节点已访问且连接到当前节点
        if (node.Row == 0) return false;
        var prevRowNodes = GetNodesAtRow(node.Row - 1);
        foreach (var prev in prevRowNodes)
        {
            if (prev.IsVisited && prev.NextNodes.Contains(id))
                return true;
        }
        return false;
    }

    public void ResetMap()
    {
        _mapNodes?.Clear();
    }
}