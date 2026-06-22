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
            default: return null;
        }
    }
}