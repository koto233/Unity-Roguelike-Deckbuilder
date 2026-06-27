using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyIntent
{
    Attack,      // 攻击
    Defend,      // 格挡
    Buff,        // 自身增益
    Debuff,      // 施加减益
    StrongAttack,// 蓄力攻击
    Unknown      // 未知/特殊
}