using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 墨影幻灵
/// </summary>
public class VantomAI : IEnemyAI
{
    private int _currentActionIndex = -1;
    /// <summary>
    /// 顺序执行
    /// </summary>
    /// <param name="ActionsIds"></param>
    /// <returns></returns> 
    public int DecideAction(int[] actionIds)
    {
        if (actionIds == null)
            throw new ArgumentNullException(nameof(actionIds));
        if (actionIds.Length == 0)
            throw new ArgumentException("动作 ID 数组不能为空。", nameof(actionIds));

        // 使用前置递增，从 -1 开始第一次变为 0
        int index = _currentActionIndex++;
        return actionIds[index % actionIds.Length];
    }

}
