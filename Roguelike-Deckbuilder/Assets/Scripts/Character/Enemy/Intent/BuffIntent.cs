using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.Config;
using UnityEngine;

public class BuffIntent : IIntent
{
    private int _buffId;
    public BuffIntent(IntentConfig config, int value)
    {
        _buffId = value;
    }
    public void Execute(EffectExecutor executor, Enemy enemy)
    {
        var config = ServiceLocator.Get<IConfigService>().GetTable<BuffConfig>().Get(_buffId);
        var buff = BuffFactory.Create(_buffId, config, 1);
        enemy.BuffManager.ApplyBuff(buff);
    }
}
