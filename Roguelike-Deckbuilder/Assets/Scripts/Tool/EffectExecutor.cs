using System;
using LitFramework;
using LitFramework.EventBus;

public class EffectExecutor
{
    private PlayerDataService _player;
    private BattleController _battleController;

    public EffectExecutor()
    {
        _player = ServiceLocator.Get<PlayerDataService>();
        _battleController = ServiceLocator.Get<BattleController>();
    }

    public void Heal(int amount, CharacterBase target)
    {
        if (target == null || amount <= 0) return;
        target.BuffManager?.OnBeforeHeal(ref amount);
        target.CurrentHp += amount;
    }
    public void HealGlobalPlayer(int amount)
    {
        _player.CurrentHp += amount;
    }
    public void Damage(int amount, EntityType attackerType, EntityType targetType, int attackerId, int targetId = 0)
    {
        if (amount <= 0) return;
        switch (targetType)
        {
            case EntityType.Player:
                var target = _battleController.Context.Player;
                var attacker = _battleController.GetEnemy(attackerId);
                Damage(amount, attacker, target);
                break;
            case EntityType.Enemy:
                Enemy targetEnemy = _battleController.GetEnemy(targetId);
                var attacker2 = _battleController.Context.Player;
                Damage(amount, attacker2, targetEnemy);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Damage(int amount, CharacterBase attacker, CharacterBase target)
    {
        if (target == null || amount <= 0) return;
        attacker.BuffManager?.OnBeforeDealDamage(ref amount);
        // 计算buff
        target.BuffManager?.OnBeforeTakeDamage(ref amount);
        // 计算护盾
        int blockAbsorb = Math.Min(target.Block, amount);
        if (blockAbsorb > 0)
        {
            target.Block -= blockAbsorb;
            amount -= blockAbsorb;
        }
        // 计算伤害
        if (amount > 0)
        {
            target.CurrentHp -= amount;
            UnityEngine.Debug.Log($"{target.EntityType} {target.CurrentHp} take damage {amount}");
        }
        EventBus<FloatingTextEvent>.Publish(new FloatingTextEvent
        {
            Text = amount.ToString(),
            EntityType = target.EntityType,
            EntityId = target.Id
        });
    }

    public void DrawCards(int count)
    {
        _battleController.Context.Player.DrawCards(count);
    }


}