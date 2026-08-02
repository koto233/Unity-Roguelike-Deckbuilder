using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyAIFactory
{
    private static readonly Dictionary<string, Func<IEnemyAI>> _aiMap = new()
    {
        ["Slime"] = () => new SlimeAI(),
        ["Brute"] = () => new BruteAI(),
        ["Default"] = () => new DefaultAI(),
    };

    public static IEnemyAI Create(string aiType)
    {
        if (_aiMap.TryGetValue(aiType, out var factory))
            return factory();

        Debug.LogWarning($"未知的 AI 类型：{aiType}，使用默认 AI");
        return new DefaultAI();
    }
}