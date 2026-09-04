using UnityEngine;

public static class BuffFactory
{
    public static IBuff Create(int id, BuffConfig config, int stacks)
    {
        Debug.Log($"创建Buff: {id}");
        return id switch
        {
            BuffIds.Vulnerable => new VulnerableBuff(config, stacks),
            BuffIds.Shrinker => new ShrinkerBuff(config, stacks),
            BuffIds.Strength => new StrengthBuff(config, stacks),
            BuffIds.Weak => new WeakBuff(config, stacks),
            _ => throw new System.NotImplementedException(),
        };
    }
}