using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.UI.Core.Service;
using UnityEngine;

public class SettingPresenter : IPresenter<UISetting>
{
    private UISetting _view;
    private UIService _uiService;
    public SettingPresenter()
    {
        _uiService = ServiceLocator.Get<UIService>();
    }
    public void Bind(UISetting view)
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
        _view.OnClickContinue += HandleClickContinue;
        _view.OnClickGiveUp += HandleClickGiveUp;
        _view.OnClickQuit += HandleClickQuit;
    }
    private void UnsubscribeEvents()
    {
        _view.OnClickContinue -= HandleClickContinue;
        _view.OnClickGiveUp -= HandleClickGiveUp;
        _view.OnClickQuit -= HandleClickQuit;
    }
    private void HandleClickContinue()
    {
        _uiService.Close<UISetting>();
    }

    private void HandleClickGiveUp()
    {
        _uiService.Close<UISetting>();
    }
    private void HandleClickQuit()
    {
        var saveService = ServiceLocator.Get<SaveService>();
        saveService.SaveGame();
        Application.Quit();
    }

}
