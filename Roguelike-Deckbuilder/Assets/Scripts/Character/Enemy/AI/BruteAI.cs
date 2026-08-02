using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// 大块头：首回防御，半血蓄力，其余攻击
public class BruteAI : IEnemyAI
{
    public IntentType DecideIntent(Enemy enemy, BattleContext context)
    {
        
        if (context.CurrentTurn == 0) return IntentType.Defend;

        float hpPercent = (float)enemy.CurrentHp / enemy.MaxHp;
        if (hpPercent < 0.5f) return IntentType.StrongAttack;

        return IntentType.Attack;
    }
}