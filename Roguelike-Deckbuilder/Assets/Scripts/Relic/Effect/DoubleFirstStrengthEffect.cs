using LitFramework;
using LitFramework.EventBus;

/// <summary>
/// 损毁头盔：你在每场战斗中第一次获得的力量值翻倍。
/// 战斗开始时标记玩家，由 BuffManager 在第一次施加力量时消费。
/// </summary>
public class DoubleFirstStrengthEffect : IRelicEffect
{
    private readonly BattleController _controller;

    public DoubleFirstStrengthEffect(RelicEffectsConfig config)
    {
        _controller = ServiceLocator.Get<BattleController>();
    }

    public void OnActivate(Relic relic)
    {
        EventBus<BattleStartedEvent>.Subscribe(OnBattleStarted);
    }

    public void OnDeactivate(Relic relic)
    {
        EventBus<BattleStartedEvent>.Unsubscribe(OnBattleStarted);

        var player = _controller.Context?.Player;
        if (player != null) player.DoubleNextStrengthGain = false;
    }

    private void OnBattleStarted(BattleStartedEvent evt)
    {
        var player = evt.Context?.Player;
        if (player != null) player.DoubleNextStrengthGain = true;
    }
}
