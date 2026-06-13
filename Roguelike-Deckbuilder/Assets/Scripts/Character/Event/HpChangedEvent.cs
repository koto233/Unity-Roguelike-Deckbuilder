using System.Collections;
using System.Collections.Generic;
using LitFramework.EventBus;
using UnityEngine;

public struct HpChangedEvent : IEvent
{
    public CharacterData characterData;
    public int OldHp;
    public int NewHp;
}
