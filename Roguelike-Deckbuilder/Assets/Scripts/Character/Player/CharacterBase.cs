using System;
using LitFramework.EventBus;
public abstract class CharacterBase
{
   protected int _currentHp;
   protected int _maxHp;
   protected int _block;
   protected int _strength;      // 力量（影响伤害）
   // public int CurrentHp => _currentHp;
   // public int MaxHp => _maxHp;
   // public int Block => _block;
   public int Strength => _strength;
   public abstract EntityType EntityType { get; }
   public abstract int Id { get; }
   private BuffManager _buffManager;
   public BuffManager BuffManager => _buffManager;
   protected CharacterBase(int maxHp)
   {
      _maxHp = maxHp;
      _currentHp = maxHp;
      _strength = 0;
      _block = 0;
      _buffManager = new BuffManager(this);
   }
   public int CurrentHp
   {
      get => _currentHp;
      set
      {
         int clamped = Math.Clamp(value, 0, MaxHp);
         if (_currentHp == clamped) return;

         int old = _currentHp;
         _currentHp = clamped;

         EventBus<HpChangedEvent>.Publish(new HpChangedEvent
         {
            OldHp = old,
            NewHp = _currentHp,
            MaxHp = _maxHp,
            EntityType = EntityType,
            EntityId = Id
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
            EntityId = Id
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
         _block = clamped;
         EventBus<BlockChangedEvent>.Publish(new BlockChangedEvent
         {
            NewBlock = _block,
            EntityType = EntityType,
            EntityId = Id
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
      _block += amount;
      EventBus<BlockChangedEvent>.Publish(new BlockChangedEvent { EntityType = EntityType, NewBlock = _block });
   }

   // 增加力量（临时/永久由调用者决定，战斗中通常临时）
   public void AddStrength(int delta)
   {
      if (delta == 0) return;
      _strength += delta;
      // EventBus<StrengthChangedEvent>.Publish(new StrengthChangedEvent { Character = this, NewStrength = _strength });
   }

   // 施加易伤
   public void ApplyVulnerable(int turns = 1)
   {
      // EventBus<BuffAppliedEvent>.Publish(new BuffAppliedEvent { Character = this, BuffType = "Vulnerable", Stacks = _vulnerable });
   }



}
public enum EntityType { Player, Enemy }