using System.Collections;
using System.Collections.Generic;
using LitFramework.EventBus;
using UnityEngine;

public struct FloatingTextEvent : IEvent
{
    public string Text;
    public Color Color;
    public bool IsCritical;
    public EntityType EntityType;
    public int EntityId;
}
