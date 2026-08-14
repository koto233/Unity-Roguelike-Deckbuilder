using System;
using Cysharp.Threading.Tasks;
using YooAsset;

namespace LitFramework.Asset
{
    public interface IAssetService
    {
        /// <summary>
        /// 同步加载资源（如非必要，推荐使用异步）
        /// </summary>
        T Load<T>(string path) where T : UnityEngine.Object;
        /// <summary>
        /// 异步加载资源，需手动管理释放
        /// </summary>
        UniTask<T> LoadAsync<T>(string path) where T : UnityEngine.Object;
        UniTask<SubAssetsHandle> LoadSubAssetsAsync<T>(string path) where T : UnityEngine.Object;

        UniTask<T> LoadSubAssetByNameAsync<T>(string path, string subName) where T : UnityEngine.Object;
        /// <summary>
        /// 增加引用计数（用于预加载或手动保持资源）
        /// </summary>
        void Retain(string path);
        /// <summary>
        /// 减少引用计数，当计数为0时卸载资源
        /// </summary>
        void Release(string path);
        /// <summary>
        /// 立即卸载指定路径的资源（忽略引用计数）
        /// </summary>
        void Unload(string path);
        /// <summary>
        /// 卸载所有引用计数为0的资源
        /// </summary>
        void ClearUnused();

        // /// <summary>
        // /// 预加载资源列表（批量增加引用计数）
        // /// </summary>
        // void Preload(string[] paths, System.Action<float> onProgress = null, Action onCompleted = null);
    }
}