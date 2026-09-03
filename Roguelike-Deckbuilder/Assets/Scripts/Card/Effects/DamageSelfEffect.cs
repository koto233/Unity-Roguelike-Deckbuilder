using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LitFramework;


public class DamageSelfEffect : CardEffectBase
{
    public DamageSelfEffect(CardEffectsConfig config, int value) : base(config, value)
    {
    }

    public override void Execute(Card card, BattleContext context)
    {
        context.Player.CurrentHp -= Value;
    }
}
