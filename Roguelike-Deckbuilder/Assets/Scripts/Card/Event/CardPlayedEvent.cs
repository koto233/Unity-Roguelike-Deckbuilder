using System.Collections;
using System.Collections.Generic;
using LitFramework.EventBus;
using UnityEngine;

public struct CardPlayedEvent : IEvent
{
    public Card Card;
}
