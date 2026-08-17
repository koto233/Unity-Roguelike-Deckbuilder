using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.UI.Core.Service;
using UnityEngine;

public class SettingPresenter : BasePresenter
{
    private UISetting _view;
    private UIService _uiService;
    public SettingPresenter(UISetting view)
    {
        _uiService = ServiceLocator.Get<UIService>();
        _view = view;
        SubscribeEvents();
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


    public override void Dispose()
    {
        UnsubscribeEvents();
    }
}
