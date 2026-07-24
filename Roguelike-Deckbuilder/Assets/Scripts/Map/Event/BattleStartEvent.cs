using LitFramework.EventBus;

public struct BattleStartEvent : IEvent
{
    public string EnemyId;
    public bool IsElite;
}