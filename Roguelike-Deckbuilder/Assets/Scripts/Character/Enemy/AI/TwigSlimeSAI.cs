using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/// <summary>
/// 树枝史莱姆（小）
/// </summary>
public class TwigSlimeSAI : IEnemyAI
{
    /// <summary>
    /// 只攻击
    /// </summary>
    /// <param name="ActionsIds"></param>
    /// <returns></returns>
    public int DecideAction(int[] ActionsIds)
    {
        return ActionsIds[0];
    }
}