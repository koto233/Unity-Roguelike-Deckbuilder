using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.EventBus;
using LitFramework.UI.Core.Service;
using UnityEngine;

public class TopBarPresenter : BasePresenter<TopBar>
{
    private UIService _uiService;
    public TopBarPresenter(TopBar view) : base(view)
    {

    }

    public override void Init()
    {
        SubscribeEvents();
        var playerDataService = ServiceLocator.Get<PlayerDataService>();
        View.RefreshHp(playerDataService.CurrentHp, playerDataService.MaxHp);
        View.RefreshCoin(0, playerDataService.Coin);
        _uiService = ServiceLocator.Get<UIService>();
    }
    private void SubscribeEvents()
    {
        View.OnClickSetting += HandleClickSetting;
        View.OnClickMap += HandleClickMap;
        View.OnClickDeck += HandleClickDeck;
        EventBus<CoinChangedEvent>.Subscribe(HandleCoinChanged);
        EventBus<HpChangedEvent>.Subscribe(HandleHpChanged);
    }



    private void UnsubscribeEvents()
    {
        View.OnClickSetting -= HandleClickSetting;
        View.OnClickMap -= HandleClickMap;
        View.OnClickDeck -= HandleClickDeck;
        EventBus<CoinChangedEvent>.Unsubscribe(HandleCoinChanged);
        EventBus<HpChangedEvent>.Unsubscribe(HandleHpChanged);
    }
    private async void HandleClickSetting()
    {
        _uiService.OpenAsync<SettingPanel>().Forget();
    }

    private void HandleClickMap()
    {
        // _uiService.OpenAsync<UIMap>().Forget();
    }
    private void HandleClickDeck()
    {
        _uiService.OpenAsync<DeckView>().Forget();
    }
    private void HandleHpChanged(HpChangedEvent @event)
    {
        if (@event.EntityType != EntityType.Player) return;
        View.RefreshHp(@event.NewHp, @event.MaxHp);
    }

    private void HandleCoinChanged(CoinChangedEvent @event)
    {
        View.RefreshCoin(@event.OldValue, @event.NewValue);
    }


    public override void Dispose()
    {
        UnsubscribeEvents();
    }


}
