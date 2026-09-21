using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LitFramework;
using LitFramework.EventBus;
using Newtonsoft.Json;

/// <summary>
/// 每当你消耗一张牌，对所有敌人造成伤害
/// </summary>
public class DamageOnExhaust : IRelicEffect
{
    private Param _param;

    public DamageOnExhaust(string paramsJson)
    {
        _param = JsonConvert.DeserializeObject<Param>(paramsJson);
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
        if (@event.Trait == CardTrait.Exhaust)
        {
            ServiceLocator.Get<EffectExecutor>().DamageAllEnemies(_param.damage);
        }
    }
    [System.Serializable]
    private class Param { public int damage; }
}
