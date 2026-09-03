using System.Collections.Generic;
using LitFramework.EventBus;


public struct RelicChangedEvent : IEvent
{
    public List<int> RelicIds;
}
