using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.UI.Core.Service;
using UnityEngine;

public class TopBarPresenter : IPresenter<UITopBar>
{
    private UITopBar _view;
    private UIService _uiService;
    public TopBarPresenter()
    {
        _uiService = ServiceLocator.Get<UIService>();
    }

    public void Bind(UITopBar view)
    {
        _view = view;
        SubscribeEvents();
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
    }
    private void UnsubscribeEvents()
    {
        _view.OnClickSetting -= HandleClickSetting;
        _view.OnClickMap -= HandleClickMap;
        _view.OnClickPile -= HandleClickPile;
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

    public void Dispose()
    {
        UnsubscribeEvents();
    }

}
