using UnityEngine;

public class VulnerableBuff : BaseBuff
{
    public VulnerableBuff(BuffConfig config, int stacks)
        : base(config, stacks) { }

    public override void OnBeforeTakeDamage(CharacterBase owner, ref int damage)
    {

        float multiplier = 1f + (Stacks * Config.Value);
        damage = Mathf.RoundToInt(damage * multiplier);
    }
}

