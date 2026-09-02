using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackIntent : IIntent
{
    private int _value;
    public AttackIntent(IntentConfig config, int value)
    {
        _value = value;
    }

    public void Execute(EffectExecutor executor, Enemy enemy)
    {
        executor.Damage(_value, EntityType.Enemy, EntityType.Player, enemy.ConfigId);
    }
}
