using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class WeakBuff : BaseBuff
{
    public WeakBuff(BuffConfig config, int stacks) : base(config, stacks)
    {
    }
    public override void OnBeforeDealDamage(CharacterBase owner, ref int damage)
    {
        if (Stacks <= 0) return;
        float DamageReduction = Config.Value / 100f;
        damage = (int)Math.Floor(damage * (1f - DamageReduction));
        base.OnBeforeDealDamage(owner, ref damage);
    }
}
