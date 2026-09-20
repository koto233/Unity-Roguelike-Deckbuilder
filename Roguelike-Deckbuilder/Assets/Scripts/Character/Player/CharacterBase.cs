using System;
using System.Collections.Generic;
using LitFramework.EventBus;
public abstract class CharacterBase
{
   protected int _currentHp;
   protected int _maxHp;
   protected int _block;
   public abstract EntityType EntityType { get; }
   private static int _nextId = 1;
   public int InstanceId { get; private set; }
   private BuffManager _buffManager;
   public BuffManager BuffManager => _buffManager;
   public float ExtraVulnerableBonus { get; set; } = 0;
   private readonly Dictionary<EffectTiming, List<Action<CharacterBase>>> _pendingEffects = new();

   public void QueueEffect(EffectTiming timing, Action<CharacterBase> effect)
   {
      if (!_pendingEffects.TryGetValue(timing, out var list))
      {
         list = new List<Action<CharacterBase>>();
         _pendingEffects[timing] = list;
      }
      list.Add(effect);
   }

   public void ExecutePendingEffects(EffectTiming timing)
   {
      if (!_pendingEffects.TryGetValue(timing, out var list)) return;

      foreach (var effect in list)
         effect(this);

      list.Clear();
   }

   public void ClearAllPendingEffects()
   {
      foreach (var list in _pendingEffects.Values)
         list.Clear();
   }
   protected CharacterBase(int maxHp)
   {
      InstanceId = _nextId++;
      _maxHp = maxHp;
      _currentHp = maxHp;
      _block = 0;
      _buffManager = new BuffManager(this);
   }
   public int CurrentHp
   {
      get => _currentHp;
      set
      {
         int oldHp = _currentHp;
         int clamped = Math.Clamp(value, 0, MaxHp);
         if (_currentHp == clamped) return;
         _currentHp = clamped;
         EventBus<HpChangedEvent>.Publish(new HpChangedEvent
         {
            OldHp = oldHp,
            NewHp = _currentHp,
            MaxHp = _maxHp,
            EntityType = EntityType,
            EntityId = InstanceId
         });

         if (_currentHp <= 0) OnDeath();
      }
   }

   public int MaxHp
   {
      get => _maxHp;
      set
      {
         if (_maxHp == value) return;
         _maxHp = value;
         EventBus<HpChangedEvent>.Publish(new HpChangedEvent
         {
            OldHp = _currentHp,
            NewHp = _currentHp,
            MaxHp = _maxHp,
            EntityType = EntityType,
            EntityId = InstanceId
         });
         // 如果当前血量超过新上限，需要截断
         if (_currentHp > _maxHp) _currentHp = _maxHp;
      }
   }

   // ===== Block：setter 自带事件 =====
   public int Block
   {
      get => _block;
      set
      {
         int clamped = Math.Max(0, value);
         if (_block == clamped) return;
         int oldBlock = _block;
         _block = clamped;
         EventBus<BlockChangedEvent>.Publish(new BlockChangedEvent
         {
            OldBlock = oldBlock,
            NewBlock = _block,
            EntityType = EntityType,
            EntityId = InstanceId
         });
      }
   }


   // public void TakeDamage(int damage)
   // {
   //    if (damage <= 0) return;
   //    _buffManager.OnBeforeTakeDamage(ref damage);
   //    int remainingDamage = damage;
   //    if (_block > 0)
   //    {
   //       int blockAbsorb = Math.Min(_block, remainingDamage);
   //       _block -= blockAbsorb;
   //       remainingDamage -= blockAbsorb;
   //       EventBus<BlockChangedEvent>.Publish(new BlockChangedEvent { NewBlock = _block, EntityType = EntityType });
   //    }

   //    if (remainingDamage > 0)
   //    {
   //       int oldHp = _currentHp;
   //       _currentHp = Math.Max(0, _currentHp - remainingDamage);
   //       if (oldHp != _currentHp)
   //       {
   //          EventBus<HpChangedEvent>.Publish(new HpChangedEvent { OldHp = oldHp, NewHp = _currentHp, MaxHp = _maxHp, EntityType = EntityType, EntityId = Id });
   //          if (_currentHp <= 0)
   //             OnDeath();
   //       }
   //    }

   //    EventBus<FloatingTextEvent>.Publish(new FloatingTextEvent { Text = damage.ToString(), IsCritical = false, EntityType = EntityType, EntityId = Id });
   // }

   // // 治疗
   // public virtual void Heal(int amount)
   // {
   //    if (amount <= 0) return;
   //    _buffManager.OnBeforeHeal(ref amount);
   //    int oldHp = _currentHp;
   //    _currentHp = Math.Min(_maxHp, _currentHp + amount);
   //    if (oldHp != _currentHp)
   //    {
   //       EventBus<HpChangedEvent>.Publish(new HpChangedEvent { OldHp = oldHp, NewHp = _currentHp, MaxHp = _maxHp, EntityType = EntityType, EntityId = Id });
   //    }

   // }


   // 回合开始时调用
   public virtual void OnTurnStart()
   {
      ClearBlock();
      ExecutePendingEffects(EffectTiming.NextTurnStart);  // 兑现
      _buffManager.OnTurnStart();
   }

   // 回合结束时清理临时效果（易伤层数减1，力量增减等）
   public virtual void OnTurnEnd()
   {
      _buffManager.OnTurnEnd();
   }
   // 使用卡牌时调用
   public virtual void OnCardPlayed(Card card)
   {
      _buffManager.OnCardPlayed(card);
   }
   // 死亡回调（由子类实现）
   protected virtual void OnDeath()
   {
      EventBus<DiedEvent>.Publish(new DiedEvent { EntityType = EntityType, Character = this });
      _buffManager.ClearAll();
   }
   // 添加 Buff 的便捷方法
   public void ApplyBuff(IBuff buff)
   {
      _buffManager.ApplyBuff(buff);
   }

   // 移除 Buff 的便捷方法
   public void RemoveBuff(IBuff buff)
   {
      _buffManager.RemoveBuff(buff);
   }

   public void RemoveBuff(int id)
   {
      _buffManager.RemoveBuffById(id);
   }

   // 检查是否有指定 Buff
   public bool HasBuff(int id)
   {
      return _buffManager.HasBuff(id);
   }

   // 获取 Buff 层数
   public int GetBuffStacks(int id)
   {
      return _buffManager.GetBuffStacks(id);
   }
   // 获得格挡
   public void AddBlock(int amount)
   {
      if (amount <= 0) return;
      Block += amount;
   }
   public void ClearBlock()
   {
      Block = 0;
   }
}
public enum EntityType { Player, Enemy }
public enum EffectTiming
{
   NextTurnStart,
   NextTurnEnd,
}