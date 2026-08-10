using System.IO;
using UnityEngine;

public class JsonSaveStorage : ISaveStorage
{
    private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    public bool HasSave() => File.Exists(SavePath);

    public void Save(string json)
    {
        File.WriteAllText(SavePath, json);
    }

    public string Load()
    {
        return File.Exists(SavePath) ? File.ReadAllText(SavePath) : null;
    }

    public void Delete()
    {
        if (File.Exists(SavePath)) File.Delete(SavePath);
    }
}