using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LitFramework;
using LitFramework.EventBus;
/// <summary>
/// 每回合第一次在回合内失去生命值时，回复等量的生命值
/// </summary>
public class HealOnFirstHpLoss : IRelicEffect
{
    private bool _triggeredThisTurn;
    private Param _param;
    public HealOnFirstHpLoss(string paramsJson)
    {


    }
    public void OnActivate(Relic relic)
    {
        EventBus<HpChangedEvent>.Subscribe(OnHpLost);
        EventBus<TurnStartEvent>.Subscribe(OnTurnStart);
    }

    public void OnDeactivate(Relic relic)
    {
        EventBus<HpChangedEvent>.Unsubscribe(OnHpLost);
        EventBus<TurnStartEvent>.Unsubscribe(OnTurnStart);
    }

    private void OnTurnStart(TurnStartEvent @event)
    {
        _triggeredThisTurn = false;
    }

    private void OnHpLost(HpChangedEvent evt)
    {
        if (evt.EntityType != EntityType.Player) return;
        if (_triggeredThisTurn) return;

        _triggeredThisTurn = true;
        int heal = Math.Abs(evt.OldHp - evt.NewHp);
        ServiceLocator.Get<EffectExecutor>().Heal(heal, EntityType.Player);
    }
    [System.Serializable]
    private class Param { public float multiplier; }
}
