using System.Collections.Generic;
using System.Linq;
using LitFramework.EventBus;
using UnityEngine;

public class BuffManager
{
    private readonly CharacterBase _owner;
    private readonly List<IBuff> _buffs = new List<IBuff>();

    public IReadOnlyList<IBuff> AllBuffs => _buffs;
    public IReadOnlyList<IBuff> Debuffs => _buffs.Where(b => b.Config.IsDebuff == 1).ToList();
    public IReadOnlyList<IBuff> Buffs => _buffs.Where(b => b.Config.IsDebuff != 1).ToList();

    public BuffManager(CharacterBase owner)
    {
        _owner = owner;
    }

    // ==================== 施加 Buff ====================
    public void ApplyBuff(IBuff buff)
    {
        var existing = _buffs.FirstOrDefault(b => b.Config.Id == buff.Config.Id);

        if (existing != null)
        {
            // ------ 不可叠层（MaxStacks == -1）------ 
            // 行为：刷新层数为新施加的层数（即重置持续时间或重置强度值）
            if (existing.Config.MaxStacks == -1)
            {
                SetStacks(existing, buff.Stacks);
                // 如果是 TurnBased 类型，刷新相当于重置倒计时；如果是 Permanent 类型，刷新相当于重置强度
                // 可选：额外触发一次 OnApply？由具体 Buff 实现决定，这里只改层数
                return;
            }

            // ------ 可叠层（MaxStacks > 0）------
            // 行为：叠加层数，但不能超过 MaxStacks
            if (existing.Stacks < existing.Config.MaxStacks)
            {
                int newStacks = Mathf.Min(existing.Stacks + buff.Stacks, existing.Config.MaxStacks);
                SetStacks(existing, newStacks); // 使用封装方法
            }
            // 如果已满，则忽略本次施加（或者也可以刷新持续时间？根据你的设计来，此处忽略）
            return;
        }

        // ------ 全新的 Buff ------
        _buffs.Add(buff);
        buff.OnApply(_owner);
        EventBus<BuffAppliedEvent>.Publish(new BuffAppliedEvent
        {
            Owner = _owner,
            Buff = buff
        });
    }
    /// <summary>
    /// 唯一修改层数的地方，自动触发 Buff 回调 + 发布 UI 刷新事件
    /// </summary>
    private void SetStacks(IBuff buff, int newStacks)
    {
        if (buff == null) return;

        int oldStacks = buff.Stacks;
        int clampedStacks = Mathf.Max(0, newStacks); // 层数不能为负

        if (Mathf.Approximately(oldStacks, clampedStacks)) return; // 没变化则不发事件

        // 1. 修改数值
        buff.Stacks = clampedStacks;

        // 2. 触发 Buff 自身的逻辑回调（如果 Buff 需要根据层数变化更新数值，在这里处理）
        buff.OnStacksChanged(_owner, oldStacks, buff.Stacks);
        Debug.Log($"Buff {buff.Config.Name} stacks changed from {oldStacks} to {buff.Stacks}");
        // 3. 发布全局事件 -> UI 订阅这个事件刷新显示
        EventBus<BuffStacksChangedEvent>.Publish(new BuffStacksChangedEvent
        {
            Owner = _owner,
            Buff = buff,
            OldStacks = oldStacks,
            NewStacks = buff.Stacks
        });

        // 4. 如果层数归零，自动移除 Buff
        if (buff.Stacks <= 0)
        {
            RemoveBuff(buff);
        }
    }
    // ==================== 移除 Buff ====================
    public void RemoveBuff(IBuff buff)
    {
        if (_buffs.Remove(buff))
        {
            buff.OnRemove(_owner);
            EventBus<BuffRemovedEvent>.Publish(new BuffRemovedEvent
            {
                Owner = _owner,
                Buff = buff
            });
        }
    }

    public void RemoveBuffById(int id)
    {
        var buff = _buffs.FirstOrDefault(b => b.Config.Id == id);
        if (buff != null) RemoveBuff(buff);
    }

    // ==================== 回合开始 ====================
    public void OnTurnStart()
    {
        foreach (var buff in _buffs.ToList())
        {
            buff.OnTurnStart(_owner);
        }
    }

    // ==================== 回合结束（核心修正） ====================
    public void OnTurnEnd()
    {
        // 1. 先触发所有 Buff 的 OnTurnEnd 回调（用于自定义逻辑，例如回合结束扣血）
        foreach (var buff in _buffs.ToList())
        {
            buff.OnTurnEnd(_owner);
        }

        // 2. 对 TurnBased 类型减少层数（层数代表剩余回合数）
        var toRemove = new List<IBuff>();
         foreach (var buff in _buffs.ToList())
        {
            if (buff.Config.DurationType == BuffDurationType.TurnBased)
            {
                SetStacks(buff, buff.Stacks - 1);

                if (buff.Stacks <= 0)
                {
                    toRemove.Add(buff);
                }
            }
            // Permanent 类型不自动减少层数
        }

        // // 3. 移除层数为 0 的 TurnBased Buff
        // foreach (var buff in toRemove)
        // {
        //     RemoveBuff(buff);
        // }

        // 4. （可选）检查 Permanent 类型的条件移除，由具体 Buff 在 OnTurnEnd 中自行调用 RemoveBuff
        // 或者你也可以增加一个通用的条件检查钩子，但为了灵活性，让 Buff 自己控制更好。
    }

    // ==================== 战斗事件钩子（保持不变） ====================
    public void OnBeforeTakeDamage(ref int damage)
    {
        foreach (var buff in _buffs)
        {
            buff.OnBeforeTakeDamage(_owner, ref damage);
        }
    }

    public void OnBeforeDealDamage(ref int damage)
    {
        foreach (var buff in _buffs)
        {
            buff.OnBeforeDealDamage(_owner, ref damage);
        }
    }

    public void OnBeforeHeal(ref int amount)
    {
        foreach (var buff in _buffs)
        {
            buff.OnBeforeHeal(_owner, ref amount);
        }
    }

    public void OnCardPlayed(Card card)
    {
        foreach (var buff in _buffs.ToList())
        {
            buff.OnCardPlayed(_owner, card);
        }
    }

    // ==================== 查询方法 ====================
    public T GetBuff<T>() where T : class, IBuff
    {
        return _buffs.FirstOrDefault(b => b is T) as T;
    }

    public int GetBuffStacks(int id)
    {
        var buff = _buffs.FirstOrDefault(b => b.Config.Id == id);
        return buff?.Stacks ?? 0;
    }

    public bool HasBuff(int id)
    {
        return _buffs.Any(b => b.Config.Id == id);
    }

    public void ClearAll()
    {
        foreach (var buff in _buffs.ToList())
        {
            buff.OnRemove(_owner);
        }
        _buffs.Clear();
    }
}