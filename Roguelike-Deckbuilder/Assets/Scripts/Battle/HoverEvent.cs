using System.Collections;
using System.Collections.Generic;
using LitFramework.EventBus;
using UnityEngine;

public struct HoverEvent : IEvent
{
    public object Data;           // Buff / Card / EnemyData
    public Vector2 ScreenPosition;
    public bool IsHovering;       // true=显示，false=隐藏
}
