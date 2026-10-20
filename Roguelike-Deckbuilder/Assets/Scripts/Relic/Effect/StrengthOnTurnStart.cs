using LitFramework;
using LitFramework.Config;
using LitFramework.EventBus;

/// <summary>
/// 硫磺：在你的每个回合开始时，你获得 X 点力量，所有敌人获得 Y 点力量。
/// </summary>
public class StrengthOnTurnStart : IRelicEffect
{
    private readonly int _playerStrength;
    private readonly int _enemyStrength;

    public StrengthOnTurnStart(RelicEffectsConfig config)
    {
        _playerStrength = config.PlayerStrength;
        _enemyStrength = config.EnemyStrength;
    }

    public void OnActivate(Relic relic)
    {
        EventBus<TurnStartEvent>.Subscribe(OnTurnStart);
    }

    public void OnDeactivate(Relic relic)
    {
        EventBus<TurnStartEvent>.Unsubscribe(OnTurnStart);
    }

    private void OnTurnStart(TurnStartEvent evt)
    {
        var context = ServiceLocator.Get<BattleController>().Context;
        if (context?.Player == null) return;

        var buffConfig = ServiceLocator.Get<IConfigService>().GetTable<BuffConfig>().Get(BuffIds.Strength);
        if (buffConfig == null) return;

        if (_playerStrength > 0)
        {
            context.Player.BuffManager.ApplyBuff(
                BuffFactory.Create(BuffIds.Strength, buffConfig, _playerStrength));
        }

        if (_enemyStrength > 0)
        {
            foreach (var enemy in context.Enemies)
            {
                enemy.BuffManager.ApplyBuff(
                    BuffFactory.Create(BuffIds.Strength, buffConfig, _enemyStrength));
            }
        }
    }
}
