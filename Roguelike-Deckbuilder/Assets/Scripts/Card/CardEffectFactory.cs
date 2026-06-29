using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardEffectFactory
{
    public static CardEffectBase Create(CardEffectsConfig config, int value)
    {
        Debug.Log("Create Card Effect: " + config.Type);
        switch (config.Type)
        {
            case "Damage": return new DamageEffect(config, value);
            case "DrawCard": return new DrawCardEffect(config, value);
            case "GainEnergy": return new GainEnergyEffect(config, value);
            case "Block": return new GainBlockEffect(config, value);
            default: return null;
        }
    }
}