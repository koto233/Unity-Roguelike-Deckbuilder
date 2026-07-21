using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WeightedRandom
{
    private static System.Random rand = new System.Random();

    public static MapNodeType PickType(MapConfig config)
    {
        // 构建权重列表（只包含非 Boss 类型，Boss 单独处理）
        var types = new List<MapNodeType> { MapNodeType.Battle, MapNodeType.Elite,
                                            MapNodeType.Rest, MapNodeType.Shop,
                                            MapNodeType.Event };
        var weights = new List<int> { config.BattleWeight, config.EliteWeight,
                                      config.RestWeight, config.ShopWeight,
                                      config.EventWeight };

        int total = 0;
        foreach (var w in weights) total += w;
        if (total == 0) return MapNodeType.Battle; // 容错

        int r = rand.Next(total);
        int cumulative = 0;
        for (int i = 0; i < types.Count; i++)
        {
            cumulative += weights[i];
            if (r < cumulative)
                return types[i];
        }
        return MapNodeType.Battle;
    }
}