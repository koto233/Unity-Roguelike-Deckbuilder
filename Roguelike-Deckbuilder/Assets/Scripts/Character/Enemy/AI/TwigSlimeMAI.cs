using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 树枝史莱姆（中）
/// </summary>
public class TwigSlimeMAI : IEnemyAI
{
    /// <summary>
    /// 随机选择一个动作
    /// </summary>
    /// <param name="ActionsIds"></param>
    /// <returns></returns>
    public int DecideAction(int[] ActionsIds)
    {
        var random = new System.Random();
        var index = random.Next(0, ActionsIds.Length);
        return ActionsIds[index];
    }
}
