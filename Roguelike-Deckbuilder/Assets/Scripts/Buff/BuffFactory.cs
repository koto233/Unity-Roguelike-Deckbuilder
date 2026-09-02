using UnityEngine;

public static class BuffFactory
{
    public static IBuff Create(int id, BuffConfig config, int stacks)
    {
        return id switch
        {
            BuffIds.Vulnerable => new VulnerableBuff(config, stacks),
            BuffIds.Shrinker => new ShrinkerBuff(config, stacks),
            _ => throw new System.NotImplementedException(),
        };
    }
}