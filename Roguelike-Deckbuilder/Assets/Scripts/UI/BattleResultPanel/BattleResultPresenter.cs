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
        // CalculateRewards();
        _rewards.Add(new Reward() { Type = RewardType.Coin, Value = _controller.Context.GoldReward });
        View.ShowReward(_rewards);
        ServiceLocator.Get<PlayerDataService>().AddCoin(_rewards[0].Value);
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