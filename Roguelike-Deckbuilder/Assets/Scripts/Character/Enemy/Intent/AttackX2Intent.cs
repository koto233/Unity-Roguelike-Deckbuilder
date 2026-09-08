using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class AttackX2Intent : IIntent
{
    private int _value;
    public AttackX2Intent(IntentConfig config, int value)
    {
        _value = value;
    }

    public void Execute(EffectExecutor executor, Enemy enemy)
    {
        for (int i = 0; i < 2; i++)
        {
            executor.Damage(_value, EntityType.Enemy, EntityType.Player, enemy.InstanceId);
            // UniTask.Delay(500);
        }
    }
}
