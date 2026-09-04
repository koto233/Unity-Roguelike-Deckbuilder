using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class StrengthBuff : BaseBuff
{
    public StrengthBuff(BuffConfig config, int stacks) : base(config, stacks)
    {

    }

    public override void OnBeforeDealDamage(CharacterBase owner, ref int damage)
    {
        damage += Stacks;
        base.OnBeforeDealDamage(owner, ref damage);
    }
}