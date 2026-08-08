using LitFramework;
using LitFramework.Asset;
using LitFramework.FSM.Procedure;
using LitFramework.UI.Core.Service;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class UITitleWindow : UIWindow
{
    private UIService uiService;

    void Start()
    {
        uiService = ServiceLocator.Get<UIService>();
        b_StartButton.onClick.AddListener(OnClickStart);
        var assetService = ServiceLocator.Get<IAssetService>();
        // assetService.LoadAsync<GameObject>("Assets/Res/UI/UICardItem.prefab", null);
    }

    private void OnClickStart()
    {
        ServiceLocator.Get<MapService>().GenerateMap(1);
        GameRoot.Instance.ProcedureManager.ChangeProcedure<ProcedureMap>();
        // StartBattleAsync().Forget();
    }

    public bool LoadGame()
    {
        var saveService = ServiceLocator.Get<SaveLoadService>();
        if (!saveService.HasSave()) return false;

        var saveData = saveService.Load();
        if (saveData == null) return false;

        // 1. 恢复玩家数据
        var playerData = ServiceLocator.Get<PlayerDataService>();
        playerData.ImportState(saveData.PlayerData);

        // 2. 恢复地图数据（重建结构 + 覆盖状态）
        var mapService = ServiceLocator.Get<MapService>();
        mapService.ImportState(saveData.MapData);

        // 3. 切换到地图流程
        var procedureManager = ServiceLocator.Get<ProcedureManager>();
        procedureManager.ChangeProcedure<ProcedureMap>();

        // 4. 刷新 UI
        var uiMap = GameObject.FindObjectOfType<UIMap>();
        uiMap?.RefreshMap(mapService.CurrentMap);

        return true;
    }

    public void SaveCurrentGame()
    {
        var mapService = ServiceLocator.Get<MapService>();
        var playerData = ServiceLocator.Get<PlayerDataService>();
        var saveService = ServiceLocator.Get<SaveLoadService>();

        var saveData = new GameSaveData
        {
            MapData = mapService.ExportSaveData(),
            PlayerData = playerData.ExportState(),
            CurrentProcedure = "Map" // 当前处于地图流程
        };
        saveService.Save(saveData);
    }
}
