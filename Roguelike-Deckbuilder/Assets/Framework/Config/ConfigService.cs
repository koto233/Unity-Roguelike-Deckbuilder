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
        private readonly Dictionary<Type, object> _tables = new();
        private IAssetService _assetManager;
        private IAssetService AssetManager => _assetManager ??= ServiceLocator.Get<IAssetService>();

        public async UniTask LoadDictTableAsync<T>(string jsonPath) where T : IConfig
        {
            var asset = await AssetManager.LoadAsync<TextAsset>(jsonPath);
            if (asset == null)
            {
                Debug.LogError($"配置表加载失败：{jsonPath}");
                return;
            }
            var dict = new Dictionary<int, T>();
            try
            {
                dict = JsonConvert.DeserializeObject<Dictionary<int, T>>(asset.text);
            }
            catch (Exception e)
            {
                Debug.LogError($"配置表格式错误：{jsonPath}");
                return;
            }
            _tables[typeof(T)] = new DictConfigTable<T>(dict);
            Debug.Log($"配置表加载成功：{jsonPath}");
        }

        public async UniTask LoadListTableAsync<T>(string jsonPath) where T : IConfig
        {
            var asset = await AssetManager.LoadAsync<TextAsset>(jsonPath);
            if (asset == null)
            {
                Debug.LogError($"配置表加载失败：{jsonPath}");
                return;
            }

            var list = JsonConvert.DeserializeObject<List<T>>(asset.text);
            _tables[typeof(T)] = new ListConfigTable<T>(list);
            Debug.Log($"配置表加载成功：{jsonPath}");
        }

        public IConfigTable<T> GetTable<T>() where T : IConfig
        {
            if (_tables.TryGetValue(typeof(T), out var obj))
                return obj as IConfigTable<T>;
            return null;
        }
    }
}
