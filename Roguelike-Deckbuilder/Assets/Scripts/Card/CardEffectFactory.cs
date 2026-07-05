using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardEffectFactory
{
    public static CardEffectBase Create(CardEffectsConfig config, int stacks)
    {
        Debug.Log("Create Card Effect: " + config.Type);
        switch (config.Type)
        {
            case "Damage": return new DamageEffect(config, stacks);
            case "DrawCard": return new DrawCardEffect(config, stacks);
            case "GainEnergy": return new GainEnergyEffect(config, stacks);
            case "Block": return new GainBlockEffect(config, stacks);
            case "Vulnerable": return new ApplyBuffEffect(config, BuffIds.Vulnerable, stacks);
            default: return null;
        }
    }
}