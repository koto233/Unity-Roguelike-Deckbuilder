using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// 默认（兜底）
public class DefaultAI : IEnemyAI
{
    public IntentType DecideIntent(Enemy enemy, BattleContext context)
    {
        return IntentType.Attack;
    }
}