// 史莱姆：只会攻击
using UnityEngine;

public class SlimeAI : IEnemyAI
{
    public IntentType DecideIntent(Enemy enemy, BattleContext context)
    {
        return IntentType.Attack; // 攻击
    }
}




