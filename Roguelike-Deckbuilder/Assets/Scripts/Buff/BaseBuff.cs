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

public enum BuffDurationType
{
    TurnBased,   // 层数代表剩余回合数，每回合-1，到0移除
    Permanent,   // 层数代表效果强度，不自动消失，需要条件移除（如驱散）
}