using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 伤害效果
public class DamageEffect : ICardEffect
{
    private int _damage;
    public DamageEffect(int damage) { _damage = damage; }
    
    public void Execute(Card card, BattleContext context)
    {
        context.Target.TakeDamage(_damage);
    }
}