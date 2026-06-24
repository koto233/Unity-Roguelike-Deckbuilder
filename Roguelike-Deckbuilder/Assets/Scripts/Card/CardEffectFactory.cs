using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardEffectFactory
{
    public static ICardEffect Create(CardEffectsConfig data)
    {
        switch (data.Type)
        {
            case "Damage": return new DamageEffect(data.Value);
            case "DrawCard": return new DrawCardEffect(data.Value);
            case "GainEnergy": return new GainEnergyEffect(data.Value);
            case "Block": return new GainBlockEffect(data.Value);
            default: return null;
        }
    }
}