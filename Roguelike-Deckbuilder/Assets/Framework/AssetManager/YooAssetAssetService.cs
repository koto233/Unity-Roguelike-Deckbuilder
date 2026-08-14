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
        // 存储 SubAssetsOperationHandle，键为资源路径
        private Dictionary<string, SubAssetsHandle> _subHandles = new();
        // 子资源引用计数
        private Dictionary<string, int> _subRefCounts = new();
        // 子资源懒加载缓存，值为 UnityEngine.Object[]，实际存储 T[]
        private Dictionary<string, AsyncLazy<UnityEngine.Object[]>> _subLazyCache = new();
        private Dictionary<string, AsyncLazy<SubAssetsHandle>> _subHandleCache = new();

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
        /// <summary>
        /// 异步加载子资源（如图集），返回操作句柄，可后续按名称查询
        /// </summary>
        public async UniTask<SubAssetsHandle> LoadSubAssetsAsync<T>(string path) where T : UnityEngine.Object
        {
            if (!_subHandleCache.TryGetValue(path, out var lazy))
            {
                lazy = new AsyncLazy<SubAssetsHandle>(async () =>
                {
                    var handle = _defaultPackage.LoadSubAssetsAsync<T>(path);
                    await handle.ToUniTask();
                    if (handle.Status != EOperationStatus.Succeeded)
                        throw new Exception($"加载子资源失败: {handle.Error}");
                    return handle;
                });
                _subHandleCache[path] = lazy;
            }

            try
            {
                var handle = await lazy.GetValueAsync();
                // 引用计数 +1
                lock (_refCounts) { _refCounts[path] = _refCounts.TryGetValue(path, out int c) ? c + 1 : 1; }
                return handle;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LoadSubAssetsAsync] 加载失败: {path}, {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 便捷方法：按名称获取单个子资源（内部调用上述加载）
        /// </summary>
        public async UniTask<T> LoadSubAssetByNameAsync<T>(string path, string subName) where T : UnityEngine.Object
        {
            var handle = await LoadSubAssetsAsync<T>(path);
            var asset = handle.GetSubAssetObject<T>(subName);
            if (asset == null)
                throw new Exception($"在路径 {path} 中未找到子资源 '{subName}'");
            return asset;
        }

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
        /// <summary>
        /// 释放指定路径的子资源（减少引用计数）
        /// </summary>
        public void ReleaseSubAssets(string path)
        {
            lock (_subRefCounts)
            {
                if (_subRefCounts.TryGetValue(path, out int count) && count > 0)
                {
                    _subRefCounts[path] = --count;
                    if (_subRefCounts[path] == 0)
                    {
                        // 释放操作句柄
                        if (_subHandles.TryGetValue(path, out var handle))
                        {
                            handle.Release();
                            _subHandles.Remove(path);
                        }
                        // 移除引用计数
                        _subRefCounts.Remove(path);
                        // 清理懒加载缓存（可选，下次加载会重新创建）
                        if (_subLazyCache.ContainsKey(path))
                            _subLazyCache.Remove(path);
                    }
                }
                else
                {
                    Debug.LogWarning($"尝试释放未加载或计数为0的子资源: {path}");
                }
            }
        }
        public void ClearUnused()
        {
            // 普通资源清理（原有逻辑）
            List<string> toRemove = new List<string>();
            foreach (var kv in _refCounts)
            {
                if (kv.Value <= 0)
                    toRemove.Add(kv.Key);
            }
            foreach (string path in toRemove)
                Unload(path);

            // 子资源清理（可选）
            List<string> subToRemove = new List<string>();
            foreach (var kv in _subRefCounts)
            {
                if (kv.Value <= 0)
                    subToRemove.Add(kv.Key);
            }
            foreach (string path in subToRemove)
                ReleaseSubAssets(path);
        }


    }


}