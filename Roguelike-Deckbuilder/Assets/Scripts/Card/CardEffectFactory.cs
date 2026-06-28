using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardEffectFactory
{
    public static ICardEffect Create(CardEffectsConfig data, int value)
    {
        Debug.Log("Create Card Effect: " + data.Type);
        switch (data.Type)
        {
            case "Damage": return new DamageEffect(value);
            case "DrawCard": return new DrawCardEffect(value);
            case "GainEnergy": return new GainEnergyEffect(value);
            case "Block": return new GainBlockEffect(value);
            default: return null;
        }
    }
}