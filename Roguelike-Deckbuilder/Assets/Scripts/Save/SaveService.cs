using System;
using LitFramework;
using LitFramework.FSM.Procedure;
using Newtonsoft.Json;
using UnityEngine;

public class SaveService
{
    private ISaveStorage _storage;
    private MapService _mapService;
    private PlayerDataService _playerData;
    private MapService MapService => _mapService ??= ServiceLocator.Get<MapService>();
    private PlayerDataService PlayerDataService => _playerData ??= ServiceLocator.Get<PlayerDataService>();
    public SaveService(ISaveStorage storage)
    {
        _storage = storage;
    }

    // ============ 存档 ============
    public void SaveGame(string currentProcedure = "Map")
    {
        var saveData = new GameSaveData
        {
            Version = 1,
            Timestamp = DateTime.Now.Ticks,
            CurrentProcedure = currentProcedure,
            MapData = MapService.ExportSaveData(),
            PlayerData = PlayerDataService.ExportState()
        };

        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        _storage.Save(json);
        Debug.Log("游戏已存档");
    }

    // ============ 读档 ============
    public bool LoadGame()
    {
        if (!_storage.HasSave()) return false;

        string json = _storage.Load();
        if (string.IsNullOrEmpty(json)) return false;

        var saveData = JsonConvert.DeserializeObject<GameSaveData>(json);
        if (saveData == null) return false;

        // 恢复数据
        PlayerDataService.ImportState(saveData.PlayerData);
        MapService.ImportState(saveData.MapData);
        ServiceLocator.Get<RelicService>().Init();
        return true;
    }

    public bool HasSave() => _storage.HasSave();
    public void DeleteSave() => _storage.Delete();
}