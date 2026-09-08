using Cysharp.Threading.Tasks;

public class AttackX3Intent : IIntent
{
    private int _value;
    public AttackX3Intent(IntentConfig config, int value)
    {
        _value = value;
    }

    public void Execute(EffectExecutor executor, Enemy enemy)
    {
        for (int i = 0; i < 3; i++)
        {
            executor.Damage(_value, EntityType.Enemy, EntityType.Player, enemy.InstanceId);
            // UniTask.Delay(500);
        }
    }
}
