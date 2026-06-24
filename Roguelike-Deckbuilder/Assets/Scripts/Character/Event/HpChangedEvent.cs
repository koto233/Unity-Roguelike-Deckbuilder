using System.Collections;
using System.Collections.Generic;
using LitFramework.EventBus;
using UnityEngine;

public struct HpChangedEvent : IEvent
{
    public EntityType EntityType;
    public int OldHp;
    public int NewHp;
    public int MaxHp;
}
