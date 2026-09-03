
using LitFramework;
using UnityEngine;
/// <summary>
/// 缩小甲虫
/// </summary>
public class BeetleAI : IEnemyAI
{
    /// <summary>
    /// 第一回合固定，后续随机
    /// </summary>
    /// <param name="ActionsIds"></param>
    /// <returns></returns>
    public int DecideAction(int[] ActionsIds)
    {
        var context = ServiceLocator.Get<BattleController>().Context;
        Debug.Log($"BeetleAI.DecideAction{context.CurrentTurn}");
        if (context.CurrentTurn <= 1)
        {
            return ActionsIds[0];
        }
        System.Random random = new();
        return ActionsIds[random.Next(1, ActionsIds.Length)];
    }
}




