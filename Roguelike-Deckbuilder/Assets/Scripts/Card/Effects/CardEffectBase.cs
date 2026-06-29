using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
public abstract class CardEffectBase
{
    public CardEffectsConfig Config { get; private set; }
    public int Value { get; private set; }

    protected CardEffectBase(CardEffectsConfig config, int value)
    {
        Config = config;
        Value = value;
    }

    public abstract void Execute(Card card, BattleContext context);
}
