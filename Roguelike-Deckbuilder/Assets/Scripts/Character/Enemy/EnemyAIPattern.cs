using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIPattern
{
    public string Condition; // 触发条件表达式，如 "Turn == 1" 或 "HpPercent < 0.5"
    public EnemyIntent Intent; // 意图类型
    public int Value; // 意图数值（攻击力/格挡量）
    public int Weight; // 权重（用于随机选择）
    public int Times; // 攻击次数（多段攻击）
    public int EffectId; // 关联的Buff/Debuff ID
    public string IntentIcon; // UI图标资源路径
}