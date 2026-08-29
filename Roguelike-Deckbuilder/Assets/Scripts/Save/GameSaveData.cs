using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    // 版本号，用于未来兼容
    public int Version = 1;
    public long Timestamp;
    // 地图数据
    public MapSaveData MapData = new();

    // 玩家数据（牌组、金币、血量等）
    public PlayerSaveData PlayerData = new();

    // 流程状态（当前在地图还是战斗中）
    public string CurrentProcedure = "Map";
}