using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.EventBus;
using LitFramework.UI.Core.Service;
using UnityEngine;

public class TopBarPresenter : IPresenter<UITopBar>
{
    private UITopBar _view;
    private UIService _uiService;

    public void Bind(UITopBar view)
    {
        _view = view;
        SubscribeEvents();
        var playerDataService = ServiceLocator.Get<PlayerDataService>();
        _view.RefreshHp(playerDataService.CurrentHp, playerDataService.MaxHp);
        _view.RefreshCoin(playerDataService.Coin);
        _uiService = ServiceLocator.Get<UIService>();
    }

    public void Unbind()
    {
        UnsubscribeEvents();
    }
    private void SubscribeEvents()
    {
        _view.OnClickSetting += HandleClickSetting;
        _view.OnClickMap += HandleClickMap;
        _view.OnClickPile += HandleClickPile;
        EventBus<CoinChangedEvent>.Subscribe(HandleCoinChanged);
        EventBus<MaxHpChangedEvent>.Subscribe(HandleMaxHpChanged);
    }



    private void UnsubscribeEvents()
    {
        _view.OnClickSetting -= HandleClickSetting;
        _view.OnClickMap -= HandleClickMap;
        _view.OnClickPile -= HandleClickPile;
        EventBus<CoinChangedEvent>.Unsubscribe(HandleCoinChanged);
        EventBus<MaxHpChangedEvent>.Unsubscribe(HandleMaxHpChanged);
    }
    private async void HandleClickSetting()
    {
        _uiService.OpenAsync<UISetting, SettingPresenter>().Forget();
    }

    private void HandleClickMap()
    {
        // _uiService.OpenAsync<UIMap>().Forget();
    }
    private void HandleClickPile()
    {

    }
    private void HandleMaxHpChanged(MaxHpChangedEvent @event)
    {
        _view.RefreshHp(@event.NewHp,@event.MaxHp);
    }

    private void HandleCoinChanged(CoinChangedEvent @event)
    {
        _view.RefreshCoin(@event.NewValue);
    }
    public void Dispose()
    {
        UnsubscribeEvents();
    }

}
