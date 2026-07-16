using System.Collections;
using System.Collections.Generic;
using LitFramework.EventBus;
using UnityEngine;

public struct FloatingTextEvent : IEvent
{
    public string Text;
    public Vector3 Position;
    public Color Color;
    public float FontSize;
    public bool IsCritical; // 暴击放大特效
}
