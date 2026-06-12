using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EffectData
{
    public EffectType EffectType;   // "Damage", "Block", "DrawCard", "GainEnergy" 等
    public int Value;           // 主要数值（伤害量、格挡值、抽牌数）
    public string Target;       // "Self", "Enemy", "AllEnemies"
}
public enum EffectType
{
    Damage,
    DrawCard,
}