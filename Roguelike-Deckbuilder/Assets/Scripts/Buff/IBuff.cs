/// <summary>
/// Buff 接口，所有 Buff/Debuff 必须实现
/// </summary>
public interface IBuff
{
    /// <summary>Buff ID，用于唯一标识和查找</summary>
    string Id { get; }
    /// <summary>
    /// 键，用于查找资源
    /// </summary> 
    string Key { get; }

    /// <summary>显示名称</summary>
    string DisplayName { get; }

    /// <summary>当前层数</summary>
    int Stacks { get; set; }

    /// <summary>剩余持续回合数（-1 表示无限）</summary>
    int Duration { get; set; }

    /// <summary>是否为负面效果（Debuff）</summary>
    bool IsDebuff { get; }


    /// <summary>描述文本（带层数占位符）</summary>
    string Description { get; }

    /// <summary>是否可以堆叠（true=多层叠加，false=刷新持续时间）</summary>
    bool CanStack { get; }

    /// <summary>最大堆叠层数（0=无限制）</summary>
    int MaxStacks { get; }

    // ==================== 生命周期回调 ====================

    /// <summary>被施加时调用</summary>
    void OnApply(CharacterBase owner);

    /// <summary>被移除时调用</summary>
    void OnRemove(CharacterBase owner);

    /// <summary>回合开始时调用</summary>
    void OnTurnStart(CharacterBase owner);

    /// <summary>回合结束时调用</summary>
    void OnTurnEnd(CharacterBase owner);

    /// <summary>受到伤害前调用（可修改伤害值）</summary>
    void OnBeforeTakeDamage(CharacterBase owner, ref int damage);

    /// <summary>造成伤害前调用（可修改伤害值）</summary>
    void OnBeforeDealDamage(CharacterBase owner, ref int damage);

    /// <summary>受到治疗前调用（可修改治疗值）</summary>
    void OnBeforeHeal(CharacterBase owner, ref int amount);

    /// <summary>使用卡牌时调用</summary>
    void OnCardPlayed(CharacterBase owner, Card card);

    /// <summary>每帧更新（用于特殊持续效果）</summary>
    void OnUpdate(CharacterBase owner, float deltaTime);

    /// <summary>层数变化时调用</summary>
    void OnStacksChanged(CharacterBase owner, int oldStacks, int newStacks);
}