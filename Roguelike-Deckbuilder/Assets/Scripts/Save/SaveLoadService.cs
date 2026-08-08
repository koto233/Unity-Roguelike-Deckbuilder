using System.IO;
using UnityEngine;
using Newtonsoft.Json; // 推荐，比 JsonUtility 更灵活

public class SaveLoadService
{
    private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    public void Save(GameSaveData data)
    {
        data.Timestamp = System.DateTime.Now.Ticks;
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(SavePath, json);
        Debug.Log($"存档已保存：{SavePath}");
    }

    public GameSaveData Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("存档文件不存在");
            return null;
        }
        string json = File.ReadAllText(SavePath);
        var data = JsonConvert.DeserializeObject<GameSaveData>(json);
        Debug.Log($"存档已加载：{SavePath}");
        return data;
    }

    public bool HasSave() => File.Exists(SavePath);
    public void DeleteSave() { if (HasSave()) File.Delete(SavePath); }
}