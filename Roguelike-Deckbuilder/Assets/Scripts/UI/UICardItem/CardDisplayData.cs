using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDisplayData
{
    public int CardId;
    public string Name;
    public int Cost;
    public string Description;
    public Sprite Icon;
    public Color CostColor;       // 已算好（能量够=白，不够=红）
    public Color RarityColor;     // 已算好
    public bool IsPlayable;       // 已算好
    public bool IsHighlighted;    // 目标选择时高亮
    public bool IsPending;        // 正在播放动画（禁止交互）
    public bool NeedTarget;
    public bool CanInteract;       // 允许交互
}