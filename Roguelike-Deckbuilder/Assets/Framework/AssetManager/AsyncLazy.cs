using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 异步延迟加载器（纯 UniTask 版）
/// 确保同一个资源的异步加载只执行一次，后续请求等待同一个结果。
/// 如果加载失败，下次请求会重新尝试加载。
/// </summary>
public class AsyncLazy<T>
{
    private readonly object _lock = new object();
    private readonly Func<UniTask<T>> _factory;

    private UniTaskCompletionSource<T> _source;
    private bool _isLoading;          // 是否正在加载（或已经加载完成）

    public AsyncLazy(Func<UniTask<T>> factory)
    {
        _factory = factory;
    }

    public UniTask<T> GetValueAsync()
    {
        // 1. 快路径：如果正在加载或已加载完成，直接返回同一个任务
        lock (_lock)
        {
            if (_isLoading)
            {
                return _source.Task;
            }
        }

        // 2. 慢路径：开始真正的加载
        lock (_lock)
        {
            // 双重检查，防止并发进入
            if (_isLoading)
            {
                return _source.Task;
            }

            // 创建新的 CompletionSource
            _source = new UniTaskCompletionSource<T>();
            _isLoading = true;
        }

        // 3. 在锁外部执行异步加载（避免长时间占用锁）
        LoadAsyncInternal();

        // 返回任务给调用者
        return _source.Task;
    }

    private async void LoadAsyncInternal()
    {
        // Debug.Log("[AsyncLazy] LoadAsyncInternal 开始"); // ⬅️
        try
        {
            // 执行工厂方法，开始真正的资源加载
            T result = await _factory();
            // Debug.Log($"[AsyncLazy] 加载成功，结果: {result}"); // ⬅️
            // 成功：设置结果
            _source.TrySetResult(result);
            // 注意：成功后 _isLoading 保持 true，以后永远返回已完成的 Task
        }
        catch (Exception ex)
        {
            // 失败：设置异常
            _source.TrySetException(ex);
            // Debug.LogError($"[AsyncLazy] 加载失败: {ex.Message}"); // ⬅️ 打印错误
            // ⭐ 关键点：失败后重置标志，允许下次调用时重新尝试加载
            lock (_lock)
            {
                _isLoading = false;
            }
        }
    }
}