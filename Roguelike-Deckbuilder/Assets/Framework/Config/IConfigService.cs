using Cysharp.Threading.Tasks;

namespace LitFramework.Config
{
    public interface IConfigService
    {
        UniTask LoadDictTableAsync<T>(string jsonPath) where T : IConfig;
        UniTask LoadListTableAsync<T>(string jsonPath) where T : IConfig;
        IConfigTable<T> GetTable<T>() where T : IConfig;
    }
}
