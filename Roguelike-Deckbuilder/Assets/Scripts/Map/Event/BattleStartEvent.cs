using LitFramework.EventBus;

public struct BattleStartEvent : IEvent
{
    public int EnemyId;
    public bool IsElite;
}