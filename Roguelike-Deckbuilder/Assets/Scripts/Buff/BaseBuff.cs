public abstract class BaseBuff : IBuff
{
    public abstract string Id { get; }
    public abstract string Key { get; }
    public abstract string DisplayName { get; }
    public virtual bool IsDebuff => false;
    public virtual string Description { get; }
    public virtual bool CanStack => true;
    public virtual int MaxStacks => 0;

    public int Stacks { get; set; } = 1;
    public int Duration { get; set; } = -1;  // -1 = 无限



    public virtual void OnApply(CharacterBase owner) { }
    public virtual void OnRemove(CharacterBase owner) { }
    public virtual void OnTurnStart(CharacterBase owner) { }
    public virtual void OnTurnEnd(CharacterBase owner) { }
    public virtual void OnBeforeTakeDamage(CharacterBase owner, ref int damage) { }
    public virtual void OnBeforeDealDamage(CharacterBase owner, ref int damage) { }
    public virtual void OnBeforeHeal(CharacterBase owner, ref int amount) { }
    public virtual void OnCardPlayed(CharacterBase owner, Card card) { }
    public virtual void OnUpdate(CharacterBase owner, float deltaTime) { }
    public virtual void OnStacksChanged(CharacterBase owner, int oldStacks, int newStacks) { }
}