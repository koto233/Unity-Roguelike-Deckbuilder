using LitFramework.EventBus;

public class BuffAppliedEvent : IEvent
{
    public CharacterBase Owner;
    public IBuff Buff;
}