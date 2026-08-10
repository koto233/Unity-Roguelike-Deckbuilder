public interface ISaveStorage
{
    bool HasSave();
    void Save(string json);
    string Load();
    void Delete();
}