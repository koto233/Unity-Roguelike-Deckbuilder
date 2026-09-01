// 史莱姆：只会攻击
using LitFramework;
using UnityEngine;

public class BeetleAI : IEnemyAI
{
    public int DecideAction(int[] ActionsIds)
    {
        var context = ServiceLocator.Get<BattleController>().Context;
        if (context.CurrentTurn == 0)
        {
            return ActionsIds[0];
        }
        System.Random random = new();
        return ActionsIds[random.Next(1, ActionsIds.Length)];
    }
}




