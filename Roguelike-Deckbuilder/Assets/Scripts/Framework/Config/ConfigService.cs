using System;
using System.Collections.Generic;
using LitFramework.AssetManager;
using UnityEngine;
namespace LitFramework.Config
{
    public class ConfigService
    {
        private Dictionary<Type, object> _tables = new();
        private IAssetManager _assetManager;
        private IAssetManager AssetManager =>
        _assetManager ??= ServiceLocator.Get<IAssetManager>();
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
                // 反序列化 JSON -> List<T>
                var list = JsonUtility.FromJson<Wrapper<T>>(asset.text)?.items;
                if (list == null)
                {
                    // 如果 JSON 是纯数组，需要用 Newtonsoft.Json 或手动处理
                    // 这里简单起见，假设 JSON 格式为 {"items":[...]}
                    Debug.LogError($"配置表格式错误：{jsonPath}");
                    onCompleted?.Invoke(false);
                    return;
                }
                var table = new DataTable<T>();
                table.Load(list);
                _tables[typeof(T)] = table;
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

    // 辅助包装类，适配 JsonUtility（因为它不支持顶级数组）
    [Serializable]
    public class Wrapper<T>
    {
        public List<T> items;
    }
}
