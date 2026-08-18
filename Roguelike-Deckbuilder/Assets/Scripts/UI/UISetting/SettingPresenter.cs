using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.UI.Core.Service;
using UnityEngine;

public class SettingPresenter : BasePresenter<UISetting>
{
    private UIService _uiService;

    public SettingPresenter(UISetting view) : base(view)
    {

    }
    public override void Init()
    {
        _uiService = ServiceLocator.Get<UIService>();
        SubscribeEvents();
    }
    private void SubscribeEvents()
    {
        View.OnClickContinue += HandleClickContinue;
        View.OnClickGiveUp += HandleClickGiveUp;
        View.OnClickQuit += HandleClickQuit;
    }
    private void UnsubscribeEvents()
    {
        View.OnClickContinue -= HandleClickContinue;
        View.OnClickGiveUp -= HandleClickGiveUp;
        View.OnClickQuit -= HandleClickQuit;
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
