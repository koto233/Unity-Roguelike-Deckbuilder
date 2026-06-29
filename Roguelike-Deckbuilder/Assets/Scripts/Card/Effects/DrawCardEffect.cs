using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardEffect : CardEffectBase
{
    public DrawCardEffect(CardEffectsConfig config, int value) : base(config, value)
    {
    }

    public override void Execute(Card card, BattleContext context)
    {
        context.Player.DrawCards(Value);
    }
}
