using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using Framework.AssetManager;
using UnityEngine;
using YooAsset;

namespace Framework.AssetManager
{
    public class YooAssetUpdater : IResourceUpdater
    {
        private string _packageName;
        private EPlayMode _playMode;
        private Action<bool> _completionCallback;
        public YooAssetUpdater(string packageName = "DefaultPackage", EPlayMode playMode = EPlayMode.EditorSimulateMode)
        {
            _packageName = packageName;
            _playMode = playMode;
        }
        public void StartUpdate(Action<bool> onCompleted)
        {
            Debug.Log($"资源系统运行模式：{_playMode}");
            _completionCallback = onCompleted;
            CoroutineRunner.Instance.RunCoroutine(UpdateRoutine());
        }
        private IEnumerator UpdateRoutine()
        {
            // 初始化 YooAsset
            YooAssets.Initialize();

            // 创建默认的资源包
            var package = YooAssets.CreatePackage(_packageName);
            var buildResult = EditorSimulateBuildInvoker.Build(_packageName, (int)EBundleType.VirtualAssetBundle);
            var packageRoot = buildResult.PackageRootDirectory;
            var fileSystemParams = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);

            var createParameters = new EditorSimulateModeOptions
            {
                EditorFileSystemParameters = fileSystemParams
            };

            var initOperation = package.InitializePackageAsync(createParameters);
            yield return initOperation;

            if (initOperation.Status == EOperationStatus.Succeeded)
            {
                Debug.Log("资源包初始化成功！");
                _completionCallback?.Invoke(true);
            }
            else
            {
                Debug.LogError($"资源包初始化失败：{initOperation.Error}");
                _completionCallback?.Invoke(false);
            }

            var reqPackageVersionOperation = package.RequestPackageVersionAsync();
            yield return reqPackageVersionOperation;
            string packageVersion = string.Empty;
            if (reqPackageVersionOperation.Status == EOperationStatus.Succeeded)
            {
                //请求成功
                packageVersion = reqPackageVersionOperation.PackageVersion;
                Debug.Log($"Request package Version : {packageVersion}");
            }
            else
            {
                //请求失败
                Debug.LogError(reqPackageVersionOperation.Error);
            }
            var loadPackageManifestOperation = package.LoadPackageManifestAsync(new LoadPackageManifestOptions(packageVersion, 60));
            yield return loadPackageManifestOperation;

            if (loadPackageManifestOperation.Status == EOperationStatus.Succeeded)
            {
                //更新成功
            }
            else
            {
                //更新失败
                Debug.LogError(loadPackageManifestOperation.Error);
            }
        }

    }


}
