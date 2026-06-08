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
            YooAssets.SetDefaultPackage(package);

            var buildResult = EditorSimulateModeHelper.SimulateBuild(_packageName);
            var packageRoot = buildResult.PackageRootDirectory;
            var fileSystemParams = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
            var createParameters = new EditorSimulateModeParameters()
            {
                EditorFileSystemParameters = fileSystemParams
            };
            InitializationOperation initializationOperation = package.InitializeAsync(createParameters);
            yield return initializationOperation;
            if (initializationOperation.Status == EOperationStatus.Succeed)

            {
                Debug.Log("资源包初始化成功！");
                _completionCallback?.Invoke(true);
            }
            else
            {
                Debug.LogError($"资源包初始化失败：{initializationOperation.Error}");
                _completionCallback?.Invoke(false);
            }

        }

    }


}
