using System.Collections;
using System.Collections.Generic;
using LitFramework.EventBus;
using UnityEngine;

public struct HpChangedEvent : IEvent
{
    public EntityType EntityType;
    public int EntityId;
    public int OldHp;
    public int NewHp;
    public int MaxHp;
}
