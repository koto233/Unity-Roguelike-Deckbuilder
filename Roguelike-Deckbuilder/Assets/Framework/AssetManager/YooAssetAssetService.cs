using System;
using System.Collections.Generic;
using LitFramework.EventBus;
using UnityEngine;
using YooAsset;
using Cysharp.Threading.Tasks;
namespace LitFramework.Asset
{
    /// <summary>
    /// YooAsset 资源管理器
    /// </summary>
    public class YooAssetAssetService : IAssetService
    {
        private ResourcePackage _defaultPackage;
        private Dictionary<string, AssetHandle> _handles = new();
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


        // public async void LoadAsync<T>(string path, Action<T> onCompleted) where T : UnityEngine.Object
        // {
        //     // 1. 如果已经加载并缓存
        //     if (_handles.ContainsKey(path))
        //     {
        //         _refCounts[path]++;
        //         onCompleted?.Invoke(_handles[path].AssetObject as T);
        //         return;
        //     }
        //     // 2. 如果正在加载中，加入等待队列
        //     if (_pendingCallbacks.ContainsKey(path))
        //     {
        //         _pendingCallbacks[path].Add(obj => onCompleted?.Invoke(obj as T));
        //         return;
        //     }
        //     // 3. 开始异步加载
        //     _pendingCallbacks[path] = new List<Action<UnityEngine.Object>>
        //     {
        //         obj => onCompleted?.Invoke(obj as T)
        //     };
        //     var handle = _defaultPackage.LoadAssetAsync<T>(path);
        //     _handles[path] = handle;   // 提前记录句柄，防止重复加载
        //     await handle;
        //     var asset = handle.AssetObject as T;
        //     // 增加引用计数
        //     _refCounts[path] = 1;
        //     // 触发所有等待回调
        //     if (_pendingCallbacks.TryGetValue(path, out var callbacks))
        //     {
        //         foreach (var cb in callbacks)
        //             cb(asset);
        //         _pendingCallbacks.Remove(path);
        //     }
        //     // 发布事件
        //     EventBus<AssetLoadedEvent>.Publish(new AssetLoadedEvent { Path = path, Asset = asset });
        // }
        public async UniTask<T> LoadAsync<T>(string path) where T : UnityEngine.Object
        {
            // 1. 如果已经加载并缓存
            if (_handles.ContainsKey(path))
            {
                _refCounts[path]++;
                return _handles[path].AssetObject as T;
            }
            // 2. 如果正在加载中，等待该任务完成
            if (_pendingTasks.TryGetValue(path, out var existingTcs))
            {
                var result = await existingTcs.Task;
                return result as T;
            }
            // 3. 首次加载 开始异步加载
            var tcs = new UniTaskCompletionSource<UnityEngine.Object>();
            _pendingTasks[path] = tcs;
            var handle = _defaultPackage.LoadAssetAsync<T>(path);
            _handles[path] = handle;
            await handle;
            var asset = handle.AssetObject;
            _refCounts[path] = 1;
            tcs.TrySetResult(asset);
            EventBus<AssetLoadedEvent>.Publish(new AssetLoadedEvent { Path = path, Asset = asset });
            _pendingTasks.Remove(path);
            return asset as T;
        }
        // public void Preload(string[] paths, Action<float> onProgress = null, Action onCompleted = null)
        // {
        //     int loaded = 0;
        //     int total = paths.Length;
        //     if (total == 0)
        //     {
        //         onCompleted?.Invoke();
        //         return;
        //     }

        //     foreach (var path in paths)
        //     {
        //         LoadAsync<UnityEngine.Object>(path, _ =>
        //         {
        //             loaded++;
        //             onProgress?.Invoke(loaded / (float)total);
        //             if (loaded >= total)
        //                 onCompleted?.Invoke();
        //         });
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