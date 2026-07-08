using System;
using LitFramework.EventBus;


public struct IntentEvent : IEvent
{
    public Enemy Enemy;
    public IntentConfig IntentConfig;
}
