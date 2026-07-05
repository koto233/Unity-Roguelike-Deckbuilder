public abstract class BaseBuff : IBuff
{
    public BuffConfig Config { get; private set; }
    public int Stacks { get; set; } = 1;
    protected BaseBuff(BuffConfig config, int stacks)
    {
        Config = config;
        Stacks = stacks;
    }


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