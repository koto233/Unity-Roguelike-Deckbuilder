using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IntentType
{
    Attack,      // 攻击
    Defend,      // 格挡
    StrongAttack,// 蓄力攻击
    MultiAttack, // 多次攻击
    Debuff,      // 施加减益
    Buff,        // 自身增益

    Unknown      // 未知/特殊
}