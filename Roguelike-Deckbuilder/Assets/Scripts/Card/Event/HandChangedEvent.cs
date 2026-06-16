using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LitFramework.EventBus;


public struct HandChangedEvent : IEvent
{
    public List<Card> Cards;
}
