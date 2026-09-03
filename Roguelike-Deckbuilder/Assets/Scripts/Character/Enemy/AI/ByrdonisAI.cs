using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LitFramework;

public class ByrdonisAI : IEnemyAI
{
    public int DecideAction(int[] ActionsIds)
    {
        var context = ServiceLocator.Get<BattleController>().Context;
        if (context.CurrentTurn <= 1)
        {
            return ActionsIds[0];
        }
        System.Random random = new();
        return ActionsIds[random.Next(0, ActionsIds.Length)];
    }
}