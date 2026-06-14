using System;
using System.Collections.Generic;
using LitFramework.Asset;
using Newtonsoft.Json;
using UnityEngine;
namespace LitFramework.Config
{
    public class ConfigService : IConfigService
    {
        private Dictionary<Type, object> _tables = new();
        private IAssetService _assetManager;
        private IAssetService AssetManager =>
        _assetManager ??= ServiceLocator.Get<IAssetService>();
        /// <summary>
        /// 加载一张配置表
        /// </summary>
        public void LoadTable<T>(string jsonPath, Action<bool> onCompleted = null) where T : IConfig
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
                // 反序列化 JSON -> List<T>
                // var list = JsonConvert.DeserializeObject<T>(asset.text);
                var dict = JsonConvert.DeserializeObject<Dictionary<string, T>>(asset.text);
                // Debug.Log($"配置表加载完成：{list.Count}");
                // if (list == null || list.Count == 0)
                // {
                //     Debug.LogError($"配置表格式错误：{jsonPath}");
                //     onCompleted?.Invoke(false);
                //     return;
                // }
                var table = new DataTable<T>();
                table.Load(dict);
                _tables[typeof(T)] = table;

                // foreach (var item in list)
                // {
                //     Debug.Log($"{item.Id} 加载完成");
                // }
                foreach (var item in dict)
                {
                    Debug.Log($"{item.Key} 加载完成");
                }
                onCompleted?.Invoke(true);
            });
        }

        public DataTable<T> GetTable<T>() where T : IConfig
        {
            if (_tables.TryGetValue(typeof(T), out var obj))
                return obj as DataTable<T>;
            return null;
        }
    }

    // // 辅助包装类，适配 JsonUtility（因为它不支持顶级数组）
    // [Serializable]
    // public class Wrapper<T>
    // {
    //     public List<T> items;
    // }
}
