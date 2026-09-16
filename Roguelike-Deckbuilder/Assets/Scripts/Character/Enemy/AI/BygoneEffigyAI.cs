using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/// <summary>
/// 旧日雕像AI
/// </summary>

public class BygoneEffigyAI : IEnemyAI
{
    private int preActionIndex = -1;
    public int DecideAction(int[] ActionsIds)
    {
        if (preActionIndex < 2)
        {
            preActionIndex++;
        }
        else
        {
            preActionIndex = 2;
        }
        return ActionsIds[preActionIndex];
    }
}
