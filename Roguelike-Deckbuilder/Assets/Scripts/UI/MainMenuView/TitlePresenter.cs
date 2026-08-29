using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.FSM.Procedure;
using LitFramework.UI.Core.Service;
using UnityEngine;

public class MainMenuPresenter : BasePresenter<MainMenuView>
{
    public MainMenuPresenter(MainMenuView view) : base(view)
    {

    }

    public override void Init()
    {
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        View.OnClickNewGame += HandleClickNewGame;
        View.OnClickContinue += HandleClickContinue;
    }
    private void UnsubscribeEvents()
    {
        View.OnClickNewGame -= HandleClickNewGame;
        View.OnClickContinue -= HandleClickContinue;
    }

    private void HandleClickNewGame()
    {
        ServiceLocator.Get<MapService>().GenerateMap(1);
        ServiceLocator.Get<ProcedureManager>().ChangeProcedure<ProcedureMap>();
        ServiceLocator.Get<PlayerDataService>().Init();
        ServiceLocator.Get<RelicService>().Init();
    }
    private void HandleClickContinue()
    {
        LoadGame();
    }

    public void LoadGame()
    {
        var saveService = ServiceLocator.Get<SaveService>();
        if (!saveService.HasSave())
        {
            return;
        }

        if (saveService.LoadGame())
        {
            // 3. 切换到地图流程
            ServiceLocator.Get<ProcedureManager>().ChangeProcedure<ProcedureMap>();
        }

    }


    public override void Dispose()
    {
        UnsubscribeEvents();
    }


}
