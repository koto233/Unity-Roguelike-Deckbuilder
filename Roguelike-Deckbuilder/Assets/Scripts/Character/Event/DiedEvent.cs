using LitFramework.EventBus;

public struct DiedEvent : IEvent
{
    public EntityType EntityType;
    public CharacterBase Character;
}