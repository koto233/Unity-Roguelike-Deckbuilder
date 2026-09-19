using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.EventBus;
using Newtonsoft.Json;
using UnityEngine;

public class ModifyVulnerableMultiplier : IRelicEffect
{
    private BattleController _battleController;
    public BattleController Controller
    {
        get
        {
            if (_battleController == null)
            {
                ServiceLocator.Get<BattleController>();
            }
            return _battleController;
        }
    }
    private Param _param;
    public ModifyVulnerableMultiplier(string paramsJson)
    {
        _param = JsonConvert.DeserializeObject<Param>(paramsJson);
    }
    public void OnActivate(Relic relic)
    {
        EventBus<BattleStartEvent>.Subscribe(OnCombatEnd);
    }



    public void OnDeactivate(Relic relic)
    {
        EventBus<BattleStartEvent>.Unsubscribe(OnCombatEnd);
    }
    private void OnCombatEnd(BattleStartEvent @event)
    {
        foreach (var enemy in Controller.Context.Enemies)
        {
            enemy.ExtraVulnerableBonus = _param.multiplier;
        }
    }


    [System.Serializable]
    private class Param { public float multiplier; }
}
