using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LitFramework.EventBus;


public struct MaxHpChangedEvent : IEvent
{
    public int OldValue;
    public int NewValue;
}
