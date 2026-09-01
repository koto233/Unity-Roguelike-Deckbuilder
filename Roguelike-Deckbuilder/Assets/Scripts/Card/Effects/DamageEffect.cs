using System.Collections;
using System.Collections.Generic;
using LitFramework;
using UnityEngine;

// 伤害效果
public class DamageEffect : CardEffectBase
{
    public DamageEffect(CardEffectsConfig config, int value) : base(config, value)
    {
    }

    public override void Execute(Card card, BattleContext context)
    {
        var executor = ServiceLocator.Get<EffectExecutor>();
        executor.Damage(Value, context.Attacker, context.Target);

    }
}