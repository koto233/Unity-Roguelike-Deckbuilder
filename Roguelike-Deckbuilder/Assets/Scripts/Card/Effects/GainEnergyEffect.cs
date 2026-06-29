using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainEnergyEffect : CardEffectBase
{
    public GainEnergyEffect(CardEffectsConfig config, int value) : base(config, value)
    {
    }

    public override void Execute(Card card, BattleContext context)
    {
        context.Player.AddEnergy(Value);
    }
}
