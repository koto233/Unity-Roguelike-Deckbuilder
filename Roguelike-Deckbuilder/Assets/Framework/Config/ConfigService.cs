using System;
using System.Collections.Generic;
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
        public void LoadDictTable<T>(string jsonPath, Action<bool> onCompleted = null) where T : IConfig
        {
            AssetManager.LoadAsync<TextAsset>(jsonPath, (asset) =>
            {
                if (asset == null)
                {
                    Debug.LogError($"配置表加载失败：{jsonPath}");
                    onCompleted?.Invoke(false);
                    return;
                }
                Debug.Log($"配置表加载完成：{asset.text}");
                var dict = JsonConvert.DeserializeObject<Dictionary<string, T>>(asset.text);
                var table = new DictConfigTable<T>(dict);
                _tables[typeof(T)] = table;
                // foreach (var item in dict)
                // {
                //     Debug.Log($"{item.Key} 加载完成");
                // }
                onCompleted?.Invoke(true);
            });
        }
        public void LoadListTable<T>(string jsonPath, Action<bool> onCompleted) where T : IConfig
        {
            AssetManager.LoadAsync<TextAsset>(jsonPath, (asset) =>
           {
               if (asset == null)
               {
                   Debug.LogError($"配置表加载失败：{jsonPath}");
                   onCompleted?.Invoke(false);
                   return;
               }
               Debug.Log($"配置表加载完成：{asset.text}");
               var list = JsonConvert.DeserializeObject<List<T>>(asset.text);
               var table = new ListConfigTable<T>(list);
               _tables[typeof(T)] = table;
               onCompleted?.Invoke(true);
           });

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
