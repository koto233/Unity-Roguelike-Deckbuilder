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
    private int _currentSeed;

    private IConfigService ConfigService =>
    _configService ??= ServiceLocator.Get<IConfigService>();
    public MapNodeData GetNode(string id) => CurrentMap?.GetValueOrDefault(id);

    public List<MapNodeData> GetNodesAtRow(int row) => CurrentMap?.Values.Where(n => n.Row == row).ToList();
    public void GenerateMap(int templateId)
    {
        _currentSeed = Random.Range(0, int.MaxValue);
        var allConfigs = ConfigService.GetTable<MapConfig>().GetAll();
        if (allConfigs == null) return;
        var rowConfigs = allConfigs
      .Where(c => c.Templateld == templateId)
      .OrderBy(c => c.Row)
      .ToList();
        CurrentMap = MapGenerator.Generate(rowConfigs, _currentSeed);
        InitStartNode();
        RefreshInteractableStates();
    }

    private void InitStartNode()
    {
        _currentNode = GetNode("0_0");
        _currentNode.IsVisited = true;
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

    // 导出地图状态（只存变动的状态，不存结构）
    public MapSaveData ExportSaveData()
    {
        return new MapSaveData
        {
            Seed = _currentSeed, // 需要你在 Generate 时保存这个种子
            CurrentNodeId = _currentNode?.Id,
            VisitedNodeIds = CurrentMap.Values.Where(n => n.IsVisited).Select(n => n.Id).ToList()
        };
    }

    // 导入地图状态（重建结构，覆盖状态）
    public void ImportState(MapSaveData saveData)
    {
        // 1. 用保存的种子重新生成地图结构
        var configs = ServiceLocator.Get<IConfigService>().GetTable<MapConfig>().GetAll()
            .Where(c => c.Templateld == 1) // 你的模板ID逻辑
            .OrderBy(c => c.Row)
            .ToList();

        CurrentMap = MapGenerator.Generate(configs, saveData.Seed);
        _currentSeed = saveData.Seed;

        // 2. 重置所有节点的访问状态
        foreach (var node in CurrentMap.Values)
        {
            node.IsVisited = false;
            node.IsLocked = node.Row > 0; // 重置锁定状态
            node.IsStart = false;
        }

        // 3. 恢复访问状态
        foreach (var id in saveData.VisitedNodeIds)
        {
            if (CurrentMap.TryGetValue(id, out var node))
            {
                node.IsVisited = true;
                // 如果是起点，特殊处理
                if (node.Row == 0) node.IsStart = true;
            }
        }

        // 4. 设置当前节点
        if (!string.IsNullOrEmpty(saveData.CurrentNodeId) && CurrentMap.TryGetValue(saveData.CurrentNodeId, out var curNode))
        {
            _currentNode = curNode;
        }
        else
        {
            // 降级：找起点
            _currentNode = CurrentMap.Values.FirstOrDefault(n => n.IsStart);
        }

        // 5. 根据已访问节点重新计算锁定状态（关键逻辑）
        RecalculateLockStates();
        RefreshInteractableStates();
    }

    private void RecalculateLockStates()
    {
        // 规则：第0行（除起点外）默认锁定；某节点被访问后，解锁其 NextNodes
        foreach (var node in CurrentMap.Values)
        {
            if (node.IsVisited || node.IsStart) continue;
            node.IsLocked = true; // 先全部锁定
        }

        // 遍历所有已访问的节点，解锁它们的下一层
        foreach (var node in CurrentMap.Values.Where(n => n.IsVisited))
        {
            foreach (var nextId in node.NextNodes)
            {
                if (CurrentMap.TryGetValue(nextId, out var next))
                    next.IsLocked = false;
            }
        }
    }
}