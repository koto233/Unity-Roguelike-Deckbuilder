using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.FSM.Procedure;
using LitFramework.UI.Core.Service;
using UnityEngine;

public class TitlePresenter : IPresenter<UITitleWindow>
{
    private UITitleWindow _view;
    public void Bind(UITitleWindow view)
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
        _view.OnClickStart += HandleClickStart;
        _view.OnClickContinue += HandleClickContinue;
    }
    private void UnsubscribeEvents()
    {
        _view.OnClickStart -= HandleClickStart;
        _view.OnClickContinue -= HandleClickContinue;
    }
    private void HandleClickStart()
    {
        ServiceLocator.Get<MapService>().GenerateMap(1);
        ServiceLocator.Get<ProcedureManager>().ChangeProcedure<ProcedureMap>();
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
}
