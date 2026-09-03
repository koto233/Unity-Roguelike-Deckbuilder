using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.FSM.Procedure;
using LitFramework.UI.Core.Service;
using UnityEngine;

public class MainMenuPresenter : BasePresenter<MainMenuView>
{
    private SaveService _saveService;
    public MainMenuPresenter(MainMenuView view) : base(view)
    {

    }

    public override void Init()
    {
        _saveService = ServiceLocator.Get<SaveService>();
        View.ShowContinue(_saveService.HasSave());
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
        ServiceLocator.Get<PlayerDataService>().Reset();
        ServiceLocator.Get<MapService>().NewMap(1);
        ServiceLocator.Get<RelicService>().Init();
        ServiceLocator.Get<ProcedureManager>().ChangeProcedure<ProcedureMap>();
        OpenTopBarAsync().Forget();
    }

    private void HandleClickContinue()
    {
        LoadGame();
    }

    public void LoadGame()
    {

        if (!_saveService.HasSave())
        {
            return;
        }
        var saveData = _saveService.LoadSaveData();
        if (saveData == null)
        {
            Debug.Log("读档失败");
            return;
        }
        ServiceLocator.Get<PlayerDataService>().Load(saveData.PlayerData);
        ServiceLocator.Get<RelicService>().Init();
        ServiceLocator.Get<MapService>().LoadMap(saveData.MapData);

        var _procedureManager = ServiceLocator.Get<ProcedureManager>();
        if (saveData.CurrentProcedure == "Map")
        {
            _procedureManager.ChangeProcedure<ProcedureMap>();
        }
        else if (saveData.CurrentProcedure == "Battle")
        {
            var args = new BattleStartParams { Type = MapNodeType.Battle };
            _procedureManager.ChangeProcedure<ProcedureBattle, BattleStartParams>(args);
        }
        OpenTopBarAsync().Forget();
    }

    private async UniTask OpenTopBarAsync()
    {
        await ServiceLocator.Get<UIService>().OpenAsync<TopBar>();
    }
    public override void Dispose()
    {
        UnsubscribeEvents();
    }


}
