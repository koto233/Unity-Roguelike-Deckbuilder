using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class GainBlockEffect : CardEffectBase
{
    public GainBlockEffect(CardEffectsConfig config, int value) : base(config, value)
    {
    }

    public override void Execute(Card card, BattleContext context)
    {
        context.Player.AddBlock(Value);
    }
}
