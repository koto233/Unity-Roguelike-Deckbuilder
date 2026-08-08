using System;
using System.Collections;
using System.Collections.Generic;
[Serializable]
public class PlayerSaveData
{
    public int Gold;
    public int MaxHp;
    public int CurrentHp;
    public List<string> DeckCardIds;     // 牌组（卡牌ID列表）
    public List<string> RelicIds;        // 遗物ID列表
}