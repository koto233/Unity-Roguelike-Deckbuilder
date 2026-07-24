using LitFramework.EventBus;

public struct BossBattleStartEvent : IEvent
{
    public string EnemyId;
}