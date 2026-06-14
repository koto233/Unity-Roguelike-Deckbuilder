using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardEffectFactory
{
    public static ICardEffect Create(CardEffects data)
    {
        switch (data.Type)
        {
            case EffectType.Damage: return new DamageEffect(data.Value);
            default: throw new Exception($"未知效果类型: {data.Type}");
        }
    }
}