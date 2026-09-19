using UnityEngine;

public class VulnerableBuff : BaseBuff
{
    public VulnerableBuff(BuffConfig config, int stacks)
        : base(config, stacks) { }

    public override void OnBeforeTakeDamage(CharacterBase owner, ref int damage)
    {
        float extra = owner.ExtraVulnerableBonus;  // 额外易伤
        float multiplier = 1f + (Stacks * Config.Value * 0.01f) + extra;
        // Debug.Log($"易伤: {multiplier} 伤害: {damage}");
        damage = Mathf.RoundToInt(damage * multiplier);
        // Debug.Log($"最终伤害: {damage}");
    }

}

