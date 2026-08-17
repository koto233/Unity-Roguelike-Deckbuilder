using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.FSM.Procedure;
using LitFramework.UI.Core.Service;
using UnityEngine;

public class TitlePresenter : BasePresenter
{
    private UITitleWindow _view;

    public TitlePresenter(UITitleWindow view)
    {
        _view = view;
        SubscribeEvents();
    }



    private void SubscribeEvents()
    {
        _view.OnClickNewGame += HandleClickNewGame;
        _view.OnClickContinue += HandleClickContinue;
    }
    private void UnsubscribeEvents()
    {
        _view.OnClickNewGame -= HandleClickNewGame;
        _view.OnClickContinue -= HandleClickContinue;
    }

    private void HandleClickNewGame()
    {
        ServiceLocator.Get<MapService>().GenerateMap(1);
        ServiceLocator.Get<ProcedureManager>().ChangeProcedure<ProcedureMap>();
        ServiceLocator.Get<PlayerDataService>().Init();
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
