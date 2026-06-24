using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class GainBlockEffect : ICardEffect
{
    private int _value;
    public GainBlockEffect(int value)
    {
        _value = value;
    }

    public void Execute(Card card, BattleContext context)
    {
        context.Player.AddBlock(_value);
    }
}
