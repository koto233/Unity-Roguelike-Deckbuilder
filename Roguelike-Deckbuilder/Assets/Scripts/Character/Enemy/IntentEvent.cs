using System;
using System.Collections.Generic;
using LitFramework.EventBus;


public struct IntentEvent : IEvent
{
    public Enemy Enemy;
    public List<IntentConfig> IntentConfigs;
}
