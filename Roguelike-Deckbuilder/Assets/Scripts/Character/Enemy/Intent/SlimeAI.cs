// 史莱姆：只会攻击
using UnityEngine;

public class SlimeAI : IEnemyAI
{
    public IntentType DecideIntent(Enemy enemy, BattleContext context)
    {
        return IntentType.Attack; // 攻击
    }
}

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


// 默认（兜底）
public class DefaultAI : IEnemyAI
{
    public IntentType DecideIntent(Enemy enemy, BattleContext context)
    {
        return IntentType.Attack;
    }
}