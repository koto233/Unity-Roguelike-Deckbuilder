using System;
using System.Collections.Generic;
using LitFramework.EventBus;


public struct IntentEvent : IEvent
{
    public int InstanceId;
    public IntentEntry[] IntentEntries;
}
