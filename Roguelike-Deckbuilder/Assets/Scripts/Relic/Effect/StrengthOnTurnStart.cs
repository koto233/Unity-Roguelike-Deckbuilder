using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LitFramework;
using LitFramework.Config;
using LitFramework.EventBus;
using Newtonsoft.Json;


public class StrengthOnTurnStart : IRelicEffect
{
    private Param _param;
    public StrengthOnTurnStart(string paramsJson)
    {
        var _param = JsonConvert.DeserializeObject<Param>(paramsJson);
    }
    public void OnActivate(Relic relic)
    {
        EventBus<BattleStartEvent>.Subscribe(OnCombatEnd);
    }


    public void OnDeactivate(Relic relic)
    {
        EventBus<BattleStartEvent>.Subscribe(OnCombatEnd);
    }

    private void OnCombatEnd(BattleStartEvent @event)
    {
        var controller = ServiceLocator.Get<BattleController>();
        var config = ServiceLocator.Get<IConfigService>().GetTable<BuffConfig>().Get(BuffIds.Strength);
        var buff = BuffFactory.Create(BuffIds.Strength, config, _param.playerStrength);
        controller.Context.Player.BuffManager.ApplyBuff(buff);
        foreach (var enemy in controller.Context.Enemies)
        {
            enemy.BuffManager.ApplyBuff(buff);
        }

    }

    [System.Serializable]
    private class Param { public int playerStrength; public int enemyStrength; }

}
