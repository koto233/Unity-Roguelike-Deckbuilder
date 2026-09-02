using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LitFramework.EventBus;


public struct BlockChangedEvent : IEvent
{
    public EntityType EntityType;
    public int EntityId;
    public int OldBlock;
    public int NewBlock;
}
