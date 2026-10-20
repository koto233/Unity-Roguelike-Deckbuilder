using LitFramework;
using LitFramework.EventBus;

/// <summary>
/// 恶魔之舌：每回合第一次在回合内失去生命值时，回复等量的生命值。
/// </summary>
public class HealOnFirstHpLoss : IRelicEffect
{
    private readonly BattleController _controller;
    private bool _triggeredThisTurn;

    public HealOnFirstHpLoss(RelicEffectsConfig config)
    {
        _controller = ServiceLocator.Get<BattleController>();
    }

    public void OnActivate(Relic relic)
    {
        EventBus<HpChangedEvent>.Subscribe(OnHpLost);
        EventBus<TurnStartEvent>.Subscribe(OnTurnStart);
        EventBus<BattleStartedEvent>.Subscribe(OnBattleStarted);
    }

    public void OnDeactivate(Relic relic)
    {
        EventBus<HpChangedEvent>.Unsubscribe(OnHpLost);
        EventBus<TurnStartEvent>.Unsubscribe(OnTurnStart);
        EventBus<BattleStartedEvent>.Unsubscribe(OnBattleStarted);
    }

    private void OnBattleStarted(BattleStartedEvent evt)
    {
        _triggeredThisTurn = false;
    }

    private void OnTurnStart(TurnStartEvent evt)
    {
        _triggeredThisTurn = false;
    }

    private void OnHpLost(HpChangedEvent evt)
    {
        var player = _controller.Context?.Player;
        if (player == null) return;
        if (evt.EntityId != player.InstanceId) return;
        if (evt.NewHp >= evt.OldHp) return; // 只处理失去生命，忽略治疗和上限变化
        if (evt.NewHp <= 0) return;         // 致命伤害不触发，避免“死后回血”
        if (_triggeredThisTurn) return;

        _triggeredThisTurn = true;
        int heal = evt.OldHp - evt.NewHp;
        ServiceLocator.Get<EffectExecutor>().Heal(heal, EntityType.Player);
    }
}
