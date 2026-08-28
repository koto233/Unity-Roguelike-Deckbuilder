using System;
using System.Collections.Generic;

public static class RelicEffectFactory
{
    private static readonly Dictionary<string, Func<string, IRelicEffect>> _creators = new()
    {
        ["DrawExtraOnTurnStart"] = p => new DrawExtraOnTurnStartEffect(p),
        // ["AddBlockOnBattleStart"] = p => new AddBlockOnBattleStartEffect(p),
        // 更多遗物效果注册...
    };

    public static IRelicEffect Create(string effectType, string paramsJson)
    {
        if (_creators.TryGetValue(effectType, out var creator))
            return creator(paramsJson);
        // Debug.LogError($"未知遗物效果类型: {effectType}");
        return null;
    }
}