using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MapChangedEvent
{
    public string ChangedNodeId; // 可选，标识哪个节点变化了，便于局部刷新
}