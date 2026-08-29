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
        var data = new GameSaveData();
        StartGame(data);
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
        var saveData = saveService.LoadSaveData();
        if (saveData == null)
        {
            Debug.Log("读档失败");
            return;
        }
        StartGame(saveData);
    }

    private void StartGame(GameSaveData data)
    {
        var playerService = ServiceLocator.Get<PlayerDataService>();
        playerService.Init(data.PlayerData);
        ServiceLocator.Get<RelicService>().Init();
        ServiceLocator.Get<MapService>().Init(data.MapData);
        // 流程切换
        ServiceLocator.Get<ProcedureManager>().ChangeProcedure<ProcedureMap>();
    }
    public override void Dispose()
    {
        UnsubscribeEvents();
    }


}
