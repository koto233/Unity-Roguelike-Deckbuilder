using LitFramework;
using LitFramework.EventBus;
using UnityEngine;

/// <summary>
/// 战斗结束恢复生命
/// </summary>
public class HealOnCombatEndEffect : IRelicEffect
{
    private int _healAmount;
    private EffectExecutor _executor;

    public HealOnCombatEndEffect(RelicEffectsConfig config)
    {
        _healAmount = config.Amount;
        _executor = ServiceLocator.Get<EffectExecutor>();
    }

    public void OnActivate(Relic relic)
    {
        EventBus<BattleEndEvent>.Subscribe(OnCombatEnd);
    }

    public void OnDeactivate(Relic relic)
    {
        EventBus<BattleEndEvent>.Unsubscribe(OnCombatEnd);
    }

    private void OnCombatEnd(BattleEndEvent evt)
    {
        Debug.Log($"OnCombatEnd{_healAmount}");
        _executor.HealGlobalPlayer(_healAmount);
    }
}
