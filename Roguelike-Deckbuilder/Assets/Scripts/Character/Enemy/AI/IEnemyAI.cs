using System.Collections.Generic;

public interface IEnemyAI
{
    /// <summary>
    /// 决定本回合使用哪个action
    /// </summary>
    /// <returns></returns>
     int DecideAction(int[] ActionsIds);
}