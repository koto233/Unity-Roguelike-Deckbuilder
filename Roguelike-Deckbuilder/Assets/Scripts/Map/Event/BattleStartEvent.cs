using System.Collections.Generic;
using LitFramework.EventBus;

public struct BattleStartEvent : IEvent
{
    public List<int> EnemyIds { get; set; }
}