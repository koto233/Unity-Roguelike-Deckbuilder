using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.EventBus;
using Newtonsoft.Json;
using UnityEngine;

public class BlockOnHpLossNextTurn : IRelicEffect
{
    private Param _param;
    public BlockOnHpLossNextTurn(string paramsJson)
    {
        _param = JsonConvert.DeserializeObject<Param>(paramsJson);
    }
    public void OnActivate(Relic relic)
    {
        EventBus<HpChangedEvent>.Subscribe(HandleHpChanged);
    }

    public void OnDeactivate(Relic relic)
    {
        EventBus<HpChangedEvent>.Unsubscribe(HandleHpChanged);
    }

    private void HandleHpChanged(HpChangedEvent @evt)
    {
        if (evt.EntityType == EntityType.Player && evt.NewHp < evt.OldHp)
        {
            ServiceLocator.Get<BattleController>().Context.Player.QueueEffect(EffectTiming.NextTurnStart, p => p.AddBlock(_param.block));
        }
    }

    [System.Serializable]
    private class Param { public int block; }
}
