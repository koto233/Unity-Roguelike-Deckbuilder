using LitFramework;
using LitFramework.Config;
using LitFramework.EventBus;

/// <summary>
/// 红头骨：当你的生命值低于或等于 x 时，额外获得 y 点力量。
/// 用单个 Buff 实例 + 实际增加层数来精确增删，避免重复叠加或误删其他力量来源。
/// </summary>
public class ThresholdStrength : IRelicEffect
{
    private readonly float _threshold;
    private readonly int _strength;
    private readonly BattleController _controller;

    private bool _active;
    private int _appliedStacks;

    public ThresholdStrength(RelicEffectsConfig config)
    {
        _threshold = config.Threshold;
        _strength = config.Strength;
        _controller = ServiceLocator.Get<BattleController>();
    }

    public void OnActivate(Relic relic)
    {
        EventBus<HpChangedEvent>.Subscribe(OnHpChanged);
        EventBus<TurnStartEvent>.Subscribe(OnTurnStart);
        EventBus<BattleStartedEvent>.Subscribe(OnBattleStarted);
    }

    public void OnDeactivate(Relic relic)
    {
        EventBus<HpChangedEvent>.Unsubscribe(OnHpChanged);
        EventBus<TurnStartEvent>.Unsubscribe(OnTurnStart);
        EventBus<BattleStartedEvent>.Unsubscribe(OnBattleStarted);

        RemoveStrength();
    }

    private void OnBattleStarted(BattleStartedEvent evt)
    {
        // 新战斗、新角色，清空上一场战斗的标记
        _active = false;
        _appliedStacks = 0;
    }

    private void OnTurnStart(TurnStartEvent evt)
    {
        UpdateState();
    }

    private void OnHpChanged(HpChangedEvent evt)
    {
        var player = GetBattlePlayer();
        if (player == null) return;
        if (evt.EntityId != player.InstanceId) return;
        if (evt.NewHp == evt.OldHp) return;

        UpdateState();
    }

    private void UpdateState()
    {
        var player = GetBattlePlayer();
        if (player == null || _strength <= 0) return;

        bool belowThreshold = player.CurrentHp <= player.MaxHp * _threshold;

        if (belowThreshold && !_active)
        {
            ApplyStrength(player);
        }
        else if (!belowThreshold && _active)
        {
            RemoveStrength();
        }
    }

    private void ApplyStrength(CharacterBase player)
    {
        var buffConfig = ServiceLocator.Get<IConfigService>().GetTable<BuffConfig>().Get(BuffIds.Strength);
        if (buffConfig == null) return;

        int before = player.GetBuffStacks(BuffIds.Strength);
        player.BuffManager.ApplyBuff(BuffFactory.Create(BuffIds.Strength, buffConfig, _strength));

        // 记录实际增加的层数：可能被 MaxStacks 截断，也可能被损毁头盔翻倍
        _appliedStacks = player.GetBuffStacks(BuffIds.Strength) - before;
        _active = true;
    }

    private void RemoveStrength()
    {
        if (!_active) return;

        var player = GetBattlePlayer();
        if (player != null && _appliedStacks > 0)
        {
            player.BuffManager.RemoveStacks(BuffIds.Strength, _appliedStacks);
        }

        _active = false;
        _appliedStacks = 0;
    }

    private CharacterBase GetBattlePlayer()
    {
        return _controller.Context?.Player;
    }
}
