using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
public class TwigSlimeS : IEnemyAI
{
    public int DecideAction(int[] ActionsIds)
    {
        return ActionsIds[0];
    }
}