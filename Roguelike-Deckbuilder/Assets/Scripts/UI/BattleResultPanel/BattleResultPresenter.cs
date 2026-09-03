using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.Config;
using LitFramework.FSM.Procedure;
using LitFramework.UI.Core.Service;
using UnityEngine;

public class BattleResultPresenter : BasePresenter<BattleResultPanel>
{
    private BattleController _controller;
    private IConfigService _configService;
    private List<Reward> _rewards = new();
    public BattleResultPresenter(BattleResultPanel view) : base(view)
    {

    }

    public override void Init()
    {
        SubscribeEvents();
        _controller = ServiceLocator.Get<BattleController>();
        CalculateRewards();
        ServiceLocator.Get<PlayerDataService>().AddCoin(_rewards[0].Value);
    }
    /// <summary>
    /// 根据敌人稀有度计算奖励
    /// </summary> 
    private void CalculateRewards()
    {
        System.Random random = new System.Random();
        _configService = ServiceLocator.Get<IConfigService>();
        var rewardConfig = _configService.GetTable<RewardConfig>();
        int rewardCoin = 0;
        foreach (var enemyConfig in _controller.Context.EnemyConfigs)
        {
            var reward = rewardConfig.Get(enemyConfig.Rarity);
            rewardCoin += random.Next(reward.CoinMin, reward.CoinMax);
        }
        _rewards.Add(new Reward() { Type = RewardType.Coin, Value = rewardCoin });
        View.ShowReward(_rewards);

    }


    private void SubscribeEvents()
    {
        View.OnSkipClick += HandleSkipClick;
        View.OnRewardItemClick += HandleRewardItemClick;
    }
    private void UnsubscribeEvents()
    {
        View.OnSkipClick -= HandleSkipClick;
        View.OnRewardItemClick -= HandleRewardItemClick;
    }

    private void HandleRewardItemClick()
    {
        // ServiceLocator.Get<PlayerDataService>().AddCoin(_rewards[0].Value);
    }

    private void HandleSkipClick()
    {
        ServiceLocator.Get<UIService>().Close<BattleResultPanel>();
        ServiceLocator.Get<ProcedureManager>().ChangeProcedure<ProcedureMap>();
    }

    public override void Dispose()
    {

        UnsubscribeEvents();
        base.Dispose();
    }


}