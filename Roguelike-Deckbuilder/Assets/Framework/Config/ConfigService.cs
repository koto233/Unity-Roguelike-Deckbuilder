using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework.Asset;
using Newtonsoft.Json;
using UnityEngine;
namespace LitFramework.Config
{
    public class ConfigService : IConfigService
    {
        private Dictionary<Type, IConfigTable> _tables = new();
        private IAssetService _assetManager;
        private IAssetService AssetManager =>
        _assetManager ??= ServiceLocator.Get<IAssetService>();



        /// <summary>
        /// 加载一张配置表为字典
        /// </summary>
        public async UniTask LoadDictTableAsync<T>(string jsonPath) where T : IConfig
        {
            TextAsset asset = await AssetManager.LoadAsync<TextAsset>(jsonPath);
            if (asset == null)
            {
                Debug.LogError($"配置表加载失败：{jsonPath}");
            }
            else
            {
                Debug.Log($"配置表加载成功：{jsonPath}");
            }
            var dict = JsonConvert.DeserializeObject<Dictionary<string, T>>(asset.text);
            var table = new DictConfigTable<T>(dict);
            _tables[typeof(T)] = table;
        }
        public async UniTask LoadListTableAsync<T>(string jsonPath) where T : IConfig
        {

            TextAsset asset = await AssetManager.LoadAsync<TextAsset>(jsonPath);
            if (asset == null)
            {
                Debug.LogError($"配置表加载失败：{jsonPath}");
            }
            else
            {
                Debug.Log($"配置表加载成功：{jsonPath}");
            }
            var list = JsonConvert.DeserializeObject<List<T>>(asset.text);
            var table = new ListConfigTable<T>(list);
            _tables[typeof(T)] = table;

        }

        public IConfigTable GetTable<T>() where T : IConfig
        {
            if (_tables.TryGetValue(typeof(T), out var obj))
                return obj;
            return null;
        }
        // public DataTable<T> GetTable<T>() where T : IConfig
        // {
        //     if (_tables.TryGetValue(typeof(T), out var obj))
        //         return obj as DataTable<T>;
        //     return null;
        // }
    }

    // // 辅助包装类，适配 JsonUtility（因为它不支持顶级数组）
    // [Serializable]
    // public class Wrapper<T>
    // {
    //     public List<T> items;
    // }
}
