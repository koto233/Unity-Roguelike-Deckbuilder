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
    public void Damage(int amount, CharacterBase target)
    {
        if (target == null || amount <= 0) return;
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