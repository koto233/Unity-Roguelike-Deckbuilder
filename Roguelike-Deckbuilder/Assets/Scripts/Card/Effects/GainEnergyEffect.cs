using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainEnergyEffect : ICardEffect
{
    private int _value;
    public GainEnergyEffect(int value)
    {
        _value = value;
    }
    public void Execute(Card card, BattleContext context)
    {
        context.Player.AddEnergy(_value);
    }
}
