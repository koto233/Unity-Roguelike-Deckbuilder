using System;
using System.Collections.Generic;
using UnityEngine;

public static class RelicEffectFactory
{
    private static readonly Dictionary<string, Func<string, IRelicEffect>> _creators = new()
    {
        ["HealOnCombatEnd"] = p => new HealOnCombatEndEffect(p),
        ["ThresholdStrength"] = p => new ThresholdStrength(p),
    };

    public static IRelicEffect Create(string effectType, string paramsJson)
    {
        if (_creators.TryGetValue(effectType, out var creator))
            return creator(paramsJson);
        Debug.LogError($"未知遗物效果类型: {effectType}");
        return null;
    }
}