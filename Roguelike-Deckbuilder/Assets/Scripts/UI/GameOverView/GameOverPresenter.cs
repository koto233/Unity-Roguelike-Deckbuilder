using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.FSM.Procedure;
using LitFramework.UI.Core.Service;
using UnityEngine;

public class GameOverPresenter : BasePresenter<GameOverView>
{
    public GameOverPresenter(GameOverView view) : base(view)
    {
    }

    public override void Init()
    {
        SubscribeEvents();
    }


    private void SubscribeEvents()
    {
        View.OnRestart += HandleRestart;
        View.OnQuit += HandleQuit;

    }


    private void UnsubscribeEvents()
    {
        View.OnRestart -= HandleRestart;
        View.OnQuit -= HandleQuit;

    }

    private void HandleQuit()
    {
        Application.Quit();
    }

    private void HandleRestart()
    {
        ServiceLocator.Get<PlayerDataService>().Reset();
        ServiceLocator.Get<MapService>().NewMap(1);
        ServiceLocator.Get<RelicService>().Init();
        ServiceLocator.Get<ProcedureManager>().ChangeProcedure<ProcedureMap>();
        OpenTopBarAsync().Forget();
    }
    private async UniTask OpenTopBarAsync()
    {
        await ServiceLocator.Get<UIService>().OpenAsync<TopBar>();
    }
    public override void Dispose()
    {
        UnsubscribeEvents();
        base.Dispose();
    }
}
