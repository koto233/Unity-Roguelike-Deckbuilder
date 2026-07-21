using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections.Generic;
using UnityEngine;

public static class MapGenerator
{
    public static List<MapNodeData> Generate(MapConfig config)
    {
        var nodes = new List<MapNodeData>();
        int rowCount = config.Row; // 行数
        int[] columnPositions = config.ColumnPositions; // 每行的列索引数组（长度=行数）

        // 1. 生成所有节点，分配类型
        for (int row = 0; row < rowCount; row++)
        {
            int colCount = columnPositions[row]; // 假设 ColumnPositions 存储的是该行的列数（或具体列索引）
            // 如果你希望 ColumnPositions 存储具体的列索引列表，则需改为 List<int[]> 结构。
            // 这里简化：假设 ColumnPositions 是每行的列数，列索引从 0 到 colCount-1。
            for (int col = 0; col < colCount; col++)
            {
                var node = new MapNodeData(row, col);
                // 最后一行强制 Boss
                if (row == rowCount - 1)
                {
                    node.Type = MapNodeType.Boss;
                    // Boss 可能只需要一个节点，所以只生成一个？但若 colCount > 1，则只取第一个为 Boss，其他可废弃？
                    // 简单处理：若最后一行有多个节点，只保留第一个作为 Boss，其余改为 Elite 或 Battle
                    if (col > 0) node.Type = MapNodeType.Elite; // 或者直接不生成
                }
                else
                {
                    node.Type = WeightedRandom.PickType(config);
                }
                // 生成敌人 ID（如果是战斗类型）
                if (node.Type == MapNodeType.Battle || node.Type == MapNodeType.Elite)
                {
                    node.EnemyId = SelectEnemyId(node.Type); // 你需要实现根据类型选择敌人ID的逻辑
                }
                nodes.Add(node);
            }
        }

        // 2. 建立行间连接（从第0行到倒数第二行）
        for (int row = 0; row < rowCount - 1; row++)
        {
            // 获取当前行和下一行的所有节点
            var currentRowNodes = nodes.FindAll(n => n.Row == row);
            var nextRowNodes = nodes.FindAll(n => n.Row == row + 1);
            if (nextRowNodes.Count == 0) break; // 安全保护

            // 为当前行每个节点添加指向下一行的连接（保证每个节点至少连一个）
            foreach (var curNode in currentRowNodes)
            {
                int col = curNode.Column;
                // 随机选择 1~3 个下一行节点，优先选择相邻索引（使路线自然）
                int count = Random.Range(1, Mathf.Min(3, nextRowNodes.Count) + 1);
                // 以当前列为中心，选取 count 个不同节点
                var candidates = new List<MapNodeData>();
                int startIdx = col - count / 2;
                for (int i = 0; i < nextRowNodes.Count; i++)
                {
                    int idx = (startIdx + i + nextRowNodes.Count) % nextRowNodes.Count;
                    candidates.Add(nextRowNodes[idx]);
                }
                // 打乱后取前 count 个
                Shuffle(candidates);
                for (int i = 0; i < count && i < candidates.Count; i++)
                {
                    if (!curNode.NextNodes.Contains(candidates[i].Id))
                        curNode.NextNodes.Add(candidates[i].Id);
                }
            }

            // 确保下一行的每个节点至少被一个上一行节点指向（连通性保证）
            foreach (var nextNode in nextRowNodes)
            {
                bool hasIncoming = false;
                foreach (var curNode in currentRowNodes)
                {
                    if (curNode.NextNodes.Contains(nextNode.Id))
                    {
                        hasIncoming = true;
                        break;
                    }
                }
                if (!hasIncoming)
                {
                    // 随机选一个当前行节点，强制添加指向 nextNode
                    var randomCur = currentRowNodes[Random.Range(0, currentRowNodes.Count)];
                    if (!randomCur.NextNodes.Contains(nextNode.Id))
                        randomCur.NextNodes.Add(nextNode.Id);
                }
            }
        }

        // 3. 标记起始节点（第0行中间列或第0列）
        var startRowNodes = nodes.FindAll(n => n.Row == 0);
        if (startRowNodes.Count > 0)
        {
            int startCol = startRowNodes.Count / 2;
            startRowNodes[startCol].IsStart = true;
            startRowNodes[startCol].IsLocked = false; // 起始解锁
        }

        return nodes;
    }

    private static void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    private static string SelectEnemyId(MapNodeType type)
    {
        // 示例：根据类型从配置中挑选敌人 ID
        // 实际应从 EnemyConfig 中根据权重或等级选取
        return type == MapNodeType.Elite ? "Elite_001" : "Battle_001";
    }
}