using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.Config;
using LitFramework.EventBus;
using Newtonsoft.Json;
using UnityEngine;
/// <summary>
/// 当你的生命值低于或等于x时，你额外获得y点力量
/// </summary>
public class ThresholdStrength : IRelicEffect
{
    private float _threshold;
    private int _strength;
    private BattleController _controller;

    public ThresholdStrength(string paramsJson)
    {
        var param = JsonConvert.DeserializeObject<Param>(paramsJson);
        _threshold = param.threshold;
        _strength = param.strength;
        _controller = ServiceLocator.Get<BattleController>();
    }
    public void OnActivate(Relic relic)
    {
        EventBus<HpChangedEvent>.Subscribe(OnHpChanged);
    }

    public void OnDeactivate(Relic relic)
    {
        EventBus<HpChangedEvent>.Unsubscribe(OnHpChanged);
    }
    private void OnHpChanged(HpChangedEvent evt)
    {
        var config = ServiceLocator.Get<IConfigService>().GetTable<BuffConfig>().Get(BuffIds.Strength);
        var buff = BuffFactory.Create(BuffIds.Strength, config, _strength);

        if (evt.EntityType == EntityType.Player)
        {
            if (evt.NewHp <= 0.5f * evt.MaxHp)
            {
                ServiceLocator.Get<BattleController>().Context.Player.BuffManager.ApplyBuff(buff);
            }
            else
            {
                ServiceLocator.Get<BattleController>().Context.Player.BuffManager.RemoveBuff(buff);
            }
        }
    }
    [System.Serializable]
    private class Param { public float threshold; public int strength; }

}
