using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class StrengthBuff : BaseBuff
{
    public int _stacks = 0;
    public StrengthBuff(BuffConfig config, int stacks) : base(config, stacks)
    {
        _stacks = stacks;
    }

    public override void OnBeforeDealDamage(CharacterBase owner, ref int damage)
    {
        damage += _stacks;
        base.OnBeforeDealDamage(owner, ref damage);
    }
}