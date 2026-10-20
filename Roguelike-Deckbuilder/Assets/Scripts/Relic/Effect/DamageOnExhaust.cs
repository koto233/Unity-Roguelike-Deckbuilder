using LitFramework;
using LitFramework.EventBus;

/// <summary>
/// 卡戎之灰：每当你消耗一张牌，对所有敌人造成伤害。
/// </summary>
public class DamageOnExhaust : IRelicEffect
{
    private readonly int _damage;

    public DamageOnExhaust(RelicEffectsConfig config)
    {
        _damage = config.Amount;
    }

    public void OnActivate(Relic relic)
    {
        EventBus<CardEvent>.Subscribe(OnCardUse);
    }

    public void OnDeactivate(Relic relic)
    {
        EventBus<CardEvent>.Unsubscribe(OnCardUse);
    }

    private void OnCardUse(CardEvent @event)
    {
        if ((@event.Trait & CardTrait.Exhaust) != 0)
        {
            ServiceLocator.Get<EffectExecutor>().DamageAllEnemies(_damage);
        }
    }
}
