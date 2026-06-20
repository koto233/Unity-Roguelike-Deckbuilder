using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework.Asset;
using UnityEngine;
namespace LitFramework.Asset
{
    public class AssetRef<T> : IDisposable where T : UnityEngine.Object
    {
        private readonly IAssetService _assetService;
        private T _asset;
        private bool _disposed;
        private readonly string _path;
        public T Asset => _asset;

        internal AssetRef(IAssetService assetService, string path, T asset)
        {
            _assetService = assetService;
            _asset = asset;
            _path = path;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (_asset != null)
            {
                _assetService.Retain(_path);
                _asset = null;
            }
        }

    }
    public static class AssetServiceExtensions
    {
        public static async UniTask<AssetRef<T>> LoadRefAsync<T>(
            this IAssetService service,
            string path) where T : UnityEngine.Object
        {
            var asset = await service.LoadAsync<T>(path);
            return new AssetRef<T>(service, path, asset);
        }
    }
}