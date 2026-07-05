using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LitFramework;
using LitFramework.Config;


public class ApplyBuffEffect : CardEffectBase
{
    private int _buffId;
    private int _stacks;
    public ApplyBuffEffect(CardEffectsConfig config, int buffId, int stacks)
        : base(config, 0)  // Value 不用了
    {
        _stacks = stacks;
        _buffId = buffId;
    }

    public override void Execute(Card card, BattleContext context)
    {
        BuffConfig config = (BuffConfig)ServiceLocator.Get<ConfigService>().GetTable<BuffConfig>().GetById(BuffIds.Vulnerable);
        var buff = BuffFactory.Create(_buffId, config, _stacks);
        context.Target.BuffManager.ApplyBuff(buff);
    }
}
