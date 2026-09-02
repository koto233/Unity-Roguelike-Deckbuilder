using System.Collections.Generic;
using LitFramework.EventBus;

public struct BattleStartEvent : IEvent
{
    public MapNodeType Type;
}