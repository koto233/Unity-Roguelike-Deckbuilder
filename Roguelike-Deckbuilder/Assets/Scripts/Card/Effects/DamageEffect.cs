using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 伤害效果
public class DamageEffect : CardEffectBase
{
    public DamageEffect(CardEffectsConfig config, int value) : base(config, value)
    {
    }

    public override void Execute(Card card, BattleContext context)
    {
        context.Target.TakeDamage(Value);
    }
}