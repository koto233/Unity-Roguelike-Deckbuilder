using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LitFramework.EventBus;


public class BlockChangedEvent : IEvent
{
    public EntityType EntityType;
    public int NewBlock;
}
