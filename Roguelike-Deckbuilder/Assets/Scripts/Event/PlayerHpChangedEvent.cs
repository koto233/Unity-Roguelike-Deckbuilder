using System.Collections;
using System.Collections.Generic;
using LitFramework.EventBus;
using UnityEngine;

public struct PlayerHpChangedEvent : IEvent
{
    public int OldHp;
    public int NewHp;
}