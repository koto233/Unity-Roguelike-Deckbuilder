using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapNodeType
{
    Battle,    // 普通战斗
    Elite,     // 精英
    Rest,      // 篝火（休息/锻造）
    Shop,      // 商店
    Treasure,  // 宝箱
    Boss       // Boss
}