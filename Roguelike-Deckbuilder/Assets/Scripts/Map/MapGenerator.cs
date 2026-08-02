using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitFramework;
using LitFramework.Config;
using UnityEngine;
public static class MapGenerator
{
    public static Dictionary<string, MapNodeData> Generate(List<MapConfig> rowConfigs, int seed = 0)
    {
        if (rowConfigs == null || rowConfigs.Count == 0)
            return null;

        var nodes = new Dictionary<string, MapNodeData>();
     
        // 1. 生成所有节点
        for (int rowIdx = 0; rowIdx < rowConfigs.Count; rowIdx++)
        {
            var rowCfg = rowConfigs[rowIdx];
            foreach (int col in rowCfg.ColumnPositions)
            {
                var node = new MapNodeData(rowIdx, col);
                // 最后一行强制 Boss（仅第一个列节点，其余可设为精英或战斗）
                if (rowIdx == rowConfigs.Count - 1)
                {
                    node.Type = (col == rowCfg.ColumnPositions[0]) ? MapNodeType.Boss : MapNodeType.Elite;
                }
                else
                {
                    node.Type = WeightedRandom.PickType(rowCfg);
                }
                // 如果是战斗类型，设置 EnemyId（此处示例）
                if (node.Type == MapNodeType.Battle || node.Type == MapNodeType.Elite || node.Type == MapNodeType.Boss)
                {
                    node.EnemyIds = new List<int> { 1 };
                }
                nodes.Add(node.Id, node);
            }
        }

        // 2. 建立行间连接（保证连通性）
        for (int row = 0; row < rowConfigs.Count - 1; row++)
        {
            var curRowNodes = nodes.Values.Where(n => n.Row == row).ToList();
            var nextRowNodes = nodes.Values.Where(n => n.Row == row + 1).ToList();
            // 为每个当前节点随机连接下一行的 1~2 个节点（确保连通）
            foreach (var cur in curRowNodes)
            {
                int count = Random.Range(1, Mathf.Min(3, nextRowNodes.Count + 1));
                // 简单随机取 count 个不同节点（需确保不重复）
                var shuffled = nextRowNodes.OrderBy(x => Random.value).Take(count).ToList();
                foreach (var next in shuffled)
                    cur.NextNodes.Add(next.Id);
            }
            // 确保下一行每个节点至少被一个上一行节点连接
            foreach (var next in nextRowNodes)
            {
                if (!curRowNodes.Any(c => c.NextNodes.Contains(next.Id)))
                {
                    var randomCur = curRowNodes[Random.Range(0, curRowNodes.Count)];
                    if (!randomCur.NextNodes.Contains(next.Id))
                        randomCur.NextNodes.Add(next.Id);
                }
            }
        }

        // 3. 标记起始节点
        if (nodes.TryGetValue("0_0", out var startRowNode))
        {
            startRowNode.IsStart = true;
            startRowNode.IsLocked = false;
        }

        return nodes;
    }
  
}