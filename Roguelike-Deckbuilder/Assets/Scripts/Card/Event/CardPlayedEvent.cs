using System.Collections;
using System.Collections.Generic;
using LitFramework.EventBus;
using UnityEngine;

public class CardPlayedEvent : IEvent
{
    public Card Card;
}
