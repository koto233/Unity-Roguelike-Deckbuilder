using System.Collections;
using System.Collections.Generic;
using LitFramework.EventBus;
using UnityEngine;

public abstract class CharacterData
{
   protected int _currentHp;
   protected int _maxHp;
   protected int _block;
   protected int _strength;      // 力量（影响伤害）
   protected int _vulnerable;    // 易伤层数（受到伤害+50%）
   public int CurrentHp => _currentHp;
   public int MaxHp => _maxHp;
   public int Block => _block;
   public int Strength => _strength;
   protected abstract EntityType EntityType { get; }
   public abstract int Id { get; }

   protected CharacterData(int maxHp, int strength = 0)
   {
      _maxHp = maxHp;
      _currentHp = maxHp;
      _strength = strength;
      _block = 0;
      _vulnerable = 0;
   }
   protected virtual void OnAfterTakeDamage(int damage) { }
   // 受到伤害（考虑格挡和易伤）
   public void TakeDamage(int damage)
   {
      if (damage <= 0) return;
      // 易伤增加伤害
      if (_vulnerable > 0)
         damage = Mathf.RoundToInt(damage * 1.5f);

      int remainingDamage = damage;
      if (_block > 0)
      {
         int blockAbsorb = Mathf.Min(_block, remainingDamage);
         _block -= blockAbsorb;
         remainingDamage -= blockAbsorb;
         EventBus<BlockChangedEvent>.Publish(new BlockChangedEvent { NewBlock = _block, EntityType = EntityType });
      }

      if (remainingDamage > 0)
      {
         int oldHp = _currentHp;
         _currentHp = Mathf.Max(0, _currentHp - remainingDamage);
         if (oldHp != _currentHp)
         {
            EventBus<HpChangedEvent>.Publish(new HpChangedEvent { OldHp = oldHp, NewHp = _currentHp, MaxHp = _maxHp, EntityType = EntityType, EntityId = Id });
            if (_currentHp <= 0)
               OnDeath();
         }
      }
   }

   // 治疗
   public virtual void Heal(int amount)
   {
      if (amount <= 0) return;
      int oldHp = _currentHp;
      _currentHp = Mathf.Min(_maxHp, _currentHp + amount);
      if (oldHp != _currentHp)
      {
         EventBus<HpChangedEvent>.Publish(new HpChangedEvent { OldHp = oldHp, NewHp = _currentHp, MaxHp = _maxHp, EntityType = EntityType, EntityId = Id });
      }

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
      _vulnerable += turns;
      // EventBus<BuffAppliedEvent>.Publish(new BuffAppliedEvent { Character = this, BuffType = "Vulnerable", Stacks = _vulnerable });
   }

   // 死亡回调（由子类实现）
   protected virtual void OnDeath()
   {
      // EventBus<CharacterDeathEvent>.Publish(new CharacterDeathEvent { Character = this });
   }

   // 回合结束时清理临时效果（易伤层数减1，力量增减等）
   public virtual void OnTurnEnd()
   {
      if (_vulnerable > 0)
      {
         _vulnerable--;
         if (_vulnerable == 0)
         {
            // EventBus<BuffExpiredEvent>.Emit(new BuffExpiredEvent { Character = this, BuffType = "Vulnerable" });
         }

      }
   }
}
