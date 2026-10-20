using LitFramework;
using LitFramework.EventBus;

/// <summary>
/// 自成型黏土：每当你在战斗中失去生命，就在下一回合获得 X 点格挡。
/// </summary>
public class BlockOnHpLossNextTurn : IRelicEffect
{
    private readonly int _block;
    private readonly BattleController _controller;

    public BlockOnHpLossNextTurn(RelicEffectsConfig config)
    {
        _block = config.Amount;
        _controller = ServiceLocator.Get<BattleController>();
    }

    public void OnActivate(Relic relic)
    {
        EventBus<HpChangedEvent>.Subscribe(HandleHpChanged);
    }

    public void OnDeactivate(Relic relic)
    {
        EventBus<HpChangedEvent>.Unsubscribe(HandleHpChanged);
    }

    private void HandleHpChanged(HpChangedEvent evt)
    {
        if (_block <= 0) return;

        var player = _controller.Context?.Player;
        if (player == null) return;
        if (evt.EntityId != player.InstanceId) return;
        if (evt.NewHp >= evt.OldHp) return;

        player.QueueEffect(EffectTiming.NextTurnStart, p => p.AddBlock(_block));
    }
}
