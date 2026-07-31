using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MapNodeData
{
    public string Id;                 // 唯一标识 "row_col"
    public int Row;                   // 行（纵轴，代表进度）
    public int Column;                // 列（横轴，代表分支）
    public MapNodeType Type;
    public int EnemyId;            // 如果是战斗节点，对应的敌人ID
    public List<string> NextNodes;    // 指向下一行的节点ID列表
    public bool IsVisited;
    public bool IsLocked = true;      // 默认锁定，只有上一行解锁了才能选
    public bool IsStart;              // 是否为起始节点
    public bool IsInteractable;
    public MapNodeData(int row, int col)
    {
        Row = row;
        Column = col;
        Id = $"{row}_{col}";
        NextNodes = new List<string>();
    }
}