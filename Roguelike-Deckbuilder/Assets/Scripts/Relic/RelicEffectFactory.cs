using System;
using System.Collections.Generic;
using UnityEngine;

public static class RelicEffectFactory
{
    private static readonly Dictionary<string, Func<string, IRelicEffect>> _creators = new()
    {
        ["HealOnCombatEnd"] = p => new HealOnCombatEndEffect(p),
        ["ThresholdStrength"] = p => new ThresholdStrength(p),
        ["StrengthOnTurnStart"] = p => new StrengthOnTurnStart(p),
        ["ModifyVulnerableMultiplier"] = p => new ModifyVulnerableMultiplier(p),
        ["BlockOnHpLossNextTurn"] = p => new BlockOnHpLossNextTurn(p),
        ["DamageOnExhaust"] = p => new DamageOnExhaust(p),
        ["HealOnFirstHpLoss"] = p => new HealOnFirstHpLoss(p),
    };

    public static IRelicEffect Create(string effectType, string paramsJson)
    {
        if (_creators.TryGetValue(effectType, out var creator))
            return creator(paramsJson);
        Debug.LogError($"未知遗物效果类型: {effectType}");
        return null;
    }
}