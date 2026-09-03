using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.UI.Core.Service;
using UnityEngine;

public class SettingPresenter : BasePresenter<SettingPanel>
{
    private UIService _uiService;

    public SettingPresenter(SettingPanel view) : base(view)
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
        _uiService.Close<SettingPanel>();
    }

    private void HandleClickGiveUp()
    {
        _uiService.Close<SettingPanel>();
    }
    private void HandleClickQuit()
    {
        var saveService = ServiceLocator.Get<SaveService>();
        var produceName = ServiceLocator.Get<ProcedureManager>().CurrentProcedure.Name;
        saveService.SaveGame(produceName);
        Application.Quit();
    }


    public override void Dispose()
    {
        UnsubscribeEvents();
        base.Dispose();
    }
}
