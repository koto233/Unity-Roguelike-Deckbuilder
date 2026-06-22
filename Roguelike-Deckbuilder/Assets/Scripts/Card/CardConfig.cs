using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardConfig : IConfig
{
    public string Id;           // 卡牌唯一ID
    public string Name;         // 显示名称
    public int Cost;            // 基础能量消耗
    public CardType Type;
    public string Description;  // 描述文本
    [NonSerialized]
    public List<CardEffectsConfig> Effects = new();  // 支持多效果（如伤害+抽牌）

    string IConfig.ID => Id;
}

public enum CardType
{
    Attack,
    Skill,
}