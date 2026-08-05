using System;
using System.Collections.Generic;
using LitFramework.EventBus;
using UnityEngine;
using YooAsset;
using Cysharp.Threading.Tasks;
using System.Linq;
namespace LitFramework.Asset
{
    /// <summary>
    /// YooAsset 资源管理器
    /// </summary>
    public class YooAssetAssetService : IAssetService
    {
        private ResourcePackage _defaultPackage;
        private Dictionary<string, AssetHandle> _handles = new();
        private readonly Dictionary<string, AsyncLazy<UnityEngine.Object>> _lazyCache = new();
        private Dictionary<string, int> _refCounts = new();
        private Dictionary<string, UniTaskCompletionSource<UnityEngine.Object>> _pendingTasks = new();

        public YooAssetAssetService(ResourcePackage package)
        {
            _defaultPackage = package;
        }
        public T Load<T>(string path) where T : UnityEngine.Object
        {
#if UNITY_EDITOR

            var handle = _defaultPackage.LoadAssetSync<T>(path);
            var asset = handle.AssetObject as T;
            handle.Release(); // 同步加载后立刻释放句柄，资源由缓存管理
            return asset;
#endif
            throw new NotSupportedException("YooAsset 正式环境推荐异步加载，请使用 LoadAsync");
        }
        public async UniTask<T> LoadAsync<T>(string path) where T : UnityEngine.Object
        {
            // 1. 获取或创建该路径的 AsyncLazy 实例
            if (!_lazyCache.TryGetValue(path, out var lazy))
            {
                lazy = new AsyncLazy<UnityEngine.Object>(async () =>
                {
                    // ⭐ 这里的加载逻辑只会在第一次调用时执行
                    // 如果加载失败，下次调用会自动重新执行
                    var handle = _defaultPackage.LoadAssetAsync<T>(path);
                    await handle.ToUniTask(); // 假设你的 YooAsset 支持转 UniTask
                    if (handle.Status != EOperationStatus.Succeeded)
                        throw new Exception($"加载失败: {handle.Error}");
                    return handle.AssetObject;
                });
                _lazyCache[path] = lazy;
            }

            // 2. 等待加载完成（如果已经加载成功，立即返回；如果加载中，等待）
            try
            {
                var asset = await lazy.GetValueAsync();
                // Debug.Log($"[LoadAsync] 成功获取资源: {path}, asset: {asset}"); // ⬅️
                lock (_refCounts)
                {
                    _refCounts.TryGetValue(path, out int count);
                    _refCounts[path] = count + 1;
                }

                // 增加引用计数等
                return asset as T;
            }
            catch (Exception ex)
            {
                // Debug.LogError($"[LoadAsync] 获取资源失败: {path}, 错误: {ex.Message}"); // ⬅️
                throw; // 或者返回 null
            }

            // 3. 增加引用计数（可选）

            // return asset as T;
        }

        // public async UniTask<T> LoadAsync<T>(string path) where T : UnityEngine.Object
        // {
        //     Debug.Log($"开始加载资源: {path}");
        //     // 1. 如果已经加载并缓存
        //     if (_handles.ContainsKey(path))
        //     {
        //         _refCounts[path]++;
        //         return _handles[path].AssetObject as T;
        //     }
        //     // 2. 如果正在加载中，等待该任务完成
        //     if (_pendingTasks.TryGetValue(path, out var existingTcs))
        //     {
        //         var result = await existingTcs.Task;
        //         return result as T;
        //     }
        //     // 3. 首次加载 开始异步加载
        //     var tcs = new UniTaskCompletionSource<UnityEngine.Object>();
        //     _pendingTasks[path] = tcs;
        //     var handle = _defaultPackage.LoadAssetAsync<T>(path);
        //     _handles[path] = handle;
        //     await handle;
        //     var asset = handle.AssetObject;
        //     _refCounts[path] = 1;
        //     tcs.TrySetResult(asset);
        //     EventBus<AssetLoadedEvent>.Publish(new AssetLoadedEvent { Path = path, Asset = asset });
        //     _pendingTasks.Remove(path);
        //     return asset as T;
        // }
        // public async UniTask PreloadAsync(string[] paths, IProgress<float> progress = null)
        // {
        //     if (paths == null || paths.Length == 0)
        //         return;

        //     // 并行启动所有加载任务
        //     var tasks = paths.Select(path => LoadAsync<UnityEngine.Object>(path)).ToList();
        //     int loaded = 0;
        //     int total = tasks.Count;

        //     // 使用 UniTask.WhenEach 逐个获取完成的任务，实时更新进度
        //     await foreach (var _ in UniTask.WhenEach(tasks))
        //     {
        //         loaded++;
        //         progress?.Report(loaded / (float)total);
        //     }
        // }

        public void Release(string path)
        {
            if (_refCounts.TryGetValue(path, out int count) && count > 0)
            {
                _refCounts[path] = --count;
                if (_refCounts[path] == 0)
                {
                    if (_handles.TryGetValue(path, out var handle))
                    {
                        handle.Release();
                        _handles.Remove(path);
                    }
                    _refCounts.Remove(path);
                }
            }
        }


        public void Retain(string path)
        {
            if (_refCounts.ContainsKey(path))
                _refCounts[path]++;
            else
                Debug.LogWarning($"尝试增加引用计数但资源未加载: {path}");
        }
        public void Unload(string path)
        {
            if (_handles.TryGetValue(path, out var handle))
            {
                handle.Release();
                _handles.Remove(path);
            }
            _refCounts.Remove(path);
            // 可选：清理等待队列（如果有）
            if (_pendingTasks.ContainsKey(path))
                _pendingTasks.Remove(path);
        }
        public void ClearUnused()
        {
            List<string> toRemove = new List<string>();
            foreach (var kv in _refCounts)
            {
                if (kv.Value <= 0)
                    toRemove.Add(kv.Key);
            }
            foreach (string path in toRemove)
                Unload(path);
        }
    }
}