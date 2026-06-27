using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

namespace LitFramework.Asset
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
        public async UniTask StartUpdate()
        {
            Debug.Log($"资源系统运行模式：{_playMode}");
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
            await initOperation; // 等待初始化完成

            if (initOperation.Status == EOperationStatus.Succeeded)
            {
                Debug.Log("资源包初始化成功！");
            }
            else
            {
                Debug.LogError($"资源包初始化失败：{initOperation.Error}");
                _completionCallback?.Invoke(false);
                // 注意：原逻辑在初始化失败后仍继续执行后续步骤，此处保持相同行为
            }

            var reqPackageVersionOperation = package.RequestPackageVersionAsync();
            await reqPackageVersionOperation;
            string packageVersion = string.Empty;
            if (reqPackageVersionOperation.Status == EOperationStatus.Succeeded)
            {
                packageVersion = reqPackageVersionOperation.PackageVersion;
                Debug.Log($"Request package Version : {packageVersion}");
            }
            else
            {
                Debug.LogError(reqPackageVersionOperation.Error);
            }

            var loadPackageManifestOperation = package.LoadPackageManifestAsync(new LoadPackageManifestOptions(packageVersion, 60));
            await loadPackageManifestOperation;

            if (loadPackageManifestOperation.Status == EOperationStatus.Succeeded)
            {
                Debug.Log($"更新成功！");
                _completionCallback?.Invoke(true);
            }
            else
            {
                Debug.LogError(loadPackageManifestOperation.Error);
            }
        }

    }


}
