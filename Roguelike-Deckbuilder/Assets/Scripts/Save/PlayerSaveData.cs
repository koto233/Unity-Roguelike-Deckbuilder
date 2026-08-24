using System;
using System.Collections;
using System.Collections.Generic;
[Serializable]
public class PlayerSaveData
{
    public int Coin;
    public int MaxHp;
    public int CurrentHp;
    public List<int> DeckCardIds;     // 牌组（卡牌ID列表）
    public List<int> RelicIds;        // 遗物ID列表
}