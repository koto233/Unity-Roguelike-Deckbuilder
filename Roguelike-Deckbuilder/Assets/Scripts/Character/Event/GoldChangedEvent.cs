using System.Collections;
using System.Collections.Generic;
using LitFramework.EventBus;
using UnityEngine;

public struct GoldChangedEvent: IEvent
{
    public int OldGold;
    public int NewGold;
}