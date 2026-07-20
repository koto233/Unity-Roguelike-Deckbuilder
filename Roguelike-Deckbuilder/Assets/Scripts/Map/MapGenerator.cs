using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapGenerator
{
    public static List<MapNodeData> Generate()
    {
        var nodes = new List<MapNodeData>();
        // 1. 循环行数（比如 4 层小怪 + 1 层 Boss）
        // 2. 每行生成 3~5 个节点，随机分配类型（根据深度加权）
        // 3. 相邻行之间建立“桥接”连接（确保每个节点至少有一条通路）
        // 4. 最后一行固定为 Boss
        return nodes;
    }
}