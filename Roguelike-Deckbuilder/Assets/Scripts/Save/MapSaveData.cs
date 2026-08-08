using System;
using System.Collections.Generic;

[Serializable]
public class MapSaveData
{
    public int Seed;                     // 用于重建地图结构
    public string CurrentNodeId;         // 当前所在节点
    public List<string> VisitedNodeIds;  // 所有已访问的节点ID
    // 注意：IsLocked 不需要存，访问节点时会自动解锁下一层，读档时重新计算即可
}