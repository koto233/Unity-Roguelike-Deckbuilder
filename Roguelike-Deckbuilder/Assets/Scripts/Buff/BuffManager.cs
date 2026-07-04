using System.Collections.Generic;
using System.Linq;
using LitFramework.EventBus;
using UnityEngine;

public class BuffManager
{
    private readonly CharacterBase _owner;
    private readonly List<IBuff> _buffs = new List<IBuff>();

    public IReadOnlyList<IBuff> AllBuffs => _buffs;
    public IReadOnlyList<IBuff> Debuffs => _buffs.Where(b => b.IsDebuff).ToList();
    public IReadOnlyList<IBuff> Buffs => _buffs.Where(b => !b.IsDebuff).ToList();

    public BuffManager(CharacterBase owner)
    {
        _owner = owner;
    }

    /// <summary>
    /// 施加 Buff
    /// </summary>
    public void ApplyBuff(IBuff buff)
    {
        // 检查是否已有同类型 Buff
        var existing = _buffs.FirstOrDefault(b => b.Id == buff.Id);

        if (existing != null)
        {
            if (existing.CanStack && existing.Stacks < existing.MaxStacks)
            {
                int oldStacks = existing.Stacks;
                existing.Stacks = Mathf.Min(existing.Stacks + buff.Stacks, existing.MaxStacks);
                existing.OnStacksChanged(_owner, oldStacks, existing.Stacks);
            }
            else
            {
                // 刷新持续时间
                existing.Duration = buff.Duration;
            }
            return;
        }

       
        // 新 Buff
        _buffs.Add(buff);
        buff.OnApply(_owner);

        EventBus<BuffAppliedEvent>.Publish(new BuffAppliedEvent
        {
            Owner = _owner,
            Buff = buff
        });
    }

    /// <summary>
    /// 移除 Buff
    /// </summary>
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

    /// <summary>
    /// 根据 ID 移除 Buff
    /// </summary>
    public void RemoveBuffById(string id)
    {
        var buff = _buffs.FirstOrDefault(b => b.Id == id);
        if (buff != null) RemoveBuff(buff);
    }

    /// <summary>
    /// 获取 Buff
    /// </summary>
    public T GetBuff<T>() where T : class, IBuff
    {
        return _buffs.FirstOrDefault(b => b is T) as T;
    }

    /// <summary>
    /// 获取 Buff 层数
    /// </summary>
    public int GetBuffStacks(string id)
    {
        var buff = _buffs.FirstOrDefault(b => b.Id == id);
        return buff?.Stacks ?? 0;
    }

    /// <summary>
    /// 检查是否有指定 Buff
    /// </summary>
    public bool HasBuff(string id)
    {
        return _buffs.Any(b => b.Id == id);
    }

    // /// <summary>
    // /// 检查是否有
    // /// </summary>
    // private bool HasArtifact()
    // {
    //     var artifact = GetBuff<ArtifactBuff>();
    //     return artifact != null && artifact.Stacks > 0;
    // }

    // /// <summary>
    // /// 消耗一层
    // /// </summary>
    // private void RemoveArtifactOneLayer()
    // {
    //     var artifact = GetBuff<ArtifactBuff>();
    //     if (artifact != null)
    //     {
    //         artifact.Stacks--;
    //         if (artifact.Stacks <= 0)
    //         {
    //             RemoveBuff(artifact);
    //         }
    //     }
    // }

    /// <summary>
    /// 回合开始：触发所有 Buff 的 OnTurnStart
    /// </summary>
    public void OnTurnStart()
    {
        // 用 ToList() 防止在遍历中修改集合
        foreach (var buff in _buffs.ToList())
        {
            buff.OnTurnStart(_owner);
        }
    }

    /// <summary>
    /// 回合结束：触发所有 Buff 的 OnTurnEnd
    /// </summary>
    public void OnTurnEnd()
    {
        foreach (var buff in _buffs.ToList())
        {
            buff.OnTurnEnd(_owner);
        }
    }

    /// <summary>
    /// 受到伤害前：触发所有 Buff 的 OnBeforeTakeDamage
    /// </summary>
    public void OnBeforeTakeDamage(ref int damage)
    {
        foreach (var buff in _buffs)
        {
            buff.OnBeforeTakeDamage(_owner, ref damage);
        }
    }

    /// <summary>
    /// 造成伤害前：触发所有 Buff 的 OnBeforeDealDamage
    /// </summary>
    public void OnBeforeDealDamage(ref int damage)
    {
        foreach (var buff in _buffs)
        {
            buff.OnBeforeDealDamage(_owner, ref damage);
        }
    }

    /// <summary>
    /// 受到治疗前：触发所有 Buff 的 OnBeforeHeal
    /// </summary>
    public void OnBeforeHeal(ref int amount)
    {
        foreach (var buff in _buffs)
        {
            buff.OnBeforeHeal(_owner, ref amount);
        }
    }

    /// <summary>
    /// 使用卡牌时：触发所有 Buff 的 OnCardPlayed
    /// </summary>
    public void OnCardPlayed(Card card)
    {
        foreach (var buff in _buffs.ToList())
        {
            buff.OnCardPlayed(_owner, card);
        }
    }

    /// <summary>
    /// 清除所有 Buff
    /// </summary>
    public void ClearAll()
    {
        foreach (var buff in _buffs.ToList())
        {
            buff.OnRemove(_owner);
        }
        _buffs.Clear();
    }
}