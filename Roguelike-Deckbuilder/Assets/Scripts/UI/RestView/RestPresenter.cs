using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LitFramework;
using LitFramework.Config;
using LitFramework.UI.Core.Service;
using UnityEngine;


public class RestPresenter : BasePresenter<RestView>
{
    private UIService _uiService;
    private int _selectedId;

    public RestPresenter(RestView view) : base(view)
    {
    }

    public override void Init()
    {
        _uiService = ServiceLocator.Get<UIService>();
        SubscribeEvents();
        View.CloseForge();
    }
    private void SubscribeEvents()
    {
        View.OnClickContinue += HandleClickContinue;
        View.OnClickForge += HandleClickForge;
        View.OnClickRest += HandleClickRest;
        View.OnConfirmClicked += OnConfirmUpgrade;
        View.OnCardSelected += OnCardSelected;
    }


    private void UnsubscribeEvents()
    {
        View.OnClickContinue -= HandleClickContinue;
        View.OnClickForge -= HandleClickForge;
        View.OnClickRest -= HandleClickRest;
        View.OnConfirmClicked -= OnConfirmUpgrade;
        View.OnCardSelected -= OnCardSelected;

    }

    private void HandleClickRest()
    {
        var globalPlayer = ServiceLocator.Get<PlayerDataService>();
        globalPlayer.CurrentHp += (int)(globalPlayer.MaxHp * .3f);
        View.ShowContinue();
    }

    private void HandleClickForge()
    {
        InitForge();
        View.OpenForge();
    }



    private void HandleClickContinue()
    {
        _uiService.Close<RestView>();
    }

    public override void Dispose()
    {
        UnsubscribeEvents();
        base.Dispose();
    }

    public void InitForge()
    {
        // 1. 从数据层取数据
        var playerService = ServiceLocator.Get<PlayerDataService>();
        var configService = ServiceLocator.Get<IConfigService>();
        var table = configService.GetTable<CardConfig>();

        var upgradeableCards = new List<Card>();
        foreach (var cardId in playerService.DeckCardIds)
        {
            var config = table.Get(cardId);
            if (config.UpgradeId > 0)
                upgradeableCards.Add(new Card(config));
        }

        // 2. 让 View 显示数据
        View.CreateUpgradeList(upgradeableCards);

        // 3. 绑定 View 的事件（Presenter 掌控一切）


        // 4. 初始状态
        _selectedId = -1;
    }

    private void OnCardSelected(int id)
    {
        _selectedId = id;
        var config = ServiceLocator.Get<IConfigService>().GetTable<CardConfig>().Get(_selectedId);
        var targetConfig = ServiceLocator.Get<IConfigService>().GetTable<CardConfig>().Get(id);
        var display = CardDisplayData.FromConfig(config);
        var targetDisplay = CardDisplayData.FromConfig(targetConfig);
        View.ShowConfirm(display, targetDisplay);
    }

    private void OnConfirmUpgrade()
    {
        if (_selectedId == -1)
        {
            Debug.LogWarning("请先选择一张卡牌");
            return;
        }

        // Presenter 唯一拥有调用 Service 的权限
        var playerService = ServiceLocator.Get<PlayerDataService>();
        var config = ServiceLocator.Get<IConfigService>().GetTable<CardConfig>().Get(_selectedId);
        playerService.UpgradeCard(config.Id, config.UpgradeId);
        View.CloseForge();
        View.ShowContinue();
    }
}