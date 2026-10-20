using System;
using System.Collections.Generic;
using UnityEngine;

public static class RelicEffectFactory
{
    private static readonly Dictionary<RelicEffectType, Func<RelicEffectsConfig, IRelicEffect>> _creators = new()
    {
        [RelicEffectType.HealOnCombatEnd] = c => new HealOnCombatEndEffect(c),
        [RelicEffectType.ThresholdStrength] = c => new ThresholdStrength(c),
        [RelicEffectType.StrengthOnTurnStart] = c => new StrengthOnTurnStart(c),
        [RelicEffectType.ModifyVulnerableMultiplier] = c => new ModifyVulnerableMultiplier(c),
        [RelicEffectType.BlockOnHpLossNextTurn] = c => new BlockOnHpLossNextTurn(c),
        [RelicEffectType.DamageOnExhaust] = c => new DamageOnExhaust(c),
        [RelicEffectType.HealOnFirstHpLoss] = c => new HealOnFirstHpLoss(c),
        [RelicEffectType.DoubleFirstStrength] = c => new DoubleFirstStrengthEffect(c),
    };

    public static IRelicEffect Create(RelicEffectsConfig config)
    {
        if (config == null) return null;

        if (_creators.TryGetValue(config.Type, out var creator))
            return creator(config);

        Debug.LogError($"未实现的遗物效果类型: {config.Type} (RelicId: {config.RelicId})");
        return null;
    }
}
