using System.Collections;
using System.Collections.Generic;
using LitFramework.EventBus;
using UnityEngine;

public struct PlayerMaxHpChangedEvent: IEvent
{
    public int NewMaxHp;
}