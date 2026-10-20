using LitFramework;
using LitFramework.EventBus;

/// <summary>
/// 纸蛙：有易伤状态的敌人受到的伤害增加 X（在易伤基础倍率上追加）。
/// 在战斗真正开始时应用到本场战斗的所有敌人。
/// </summary>
public class ModifyVulnerableMultiplier : IRelicEffect
{
    private readonly BattleController _controller;
    private readonly float _multiplier;

    public ModifyVulnerableMultiplier(RelicEffectsConfig config)
    {
        _controller = ServiceLocator.Get<BattleController>();
        _multiplier = config.Multiplier;
    }

    public void OnActivate(Relic relic)
    {
        EventBus<BattleStartedEvent>.Subscribe(OnBattleStarted);
    }

    public void OnDeactivate(Relic relic)
    {
        EventBus<BattleStartedEvent>.Unsubscribe(OnBattleStarted);
        ClearBonus();
    }

    private void OnBattleStarted(BattleStartedEvent evt)
    {
        if (evt.Context == null) return;

        foreach (var enemy in evt.Context.Enemies)
        {
            enemy.ExtraVulnerableBonus = _multiplier;
        }
    }

    private void ClearBonus()
    {
        var context = _controller.Context;
        if (context == null) return;

        foreach (var enemy in context.Enemies)
        {
            enemy.ExtraVulnerableBonus = 0;
        }
    }
}
