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
        private FileSystemParameters _fileSystemParams;
        public YooAssetUpdater(string packageName = "DefaultPackage", EPlayMode playMode = EPlayMode.EditorSimulateMode)
        {
            _packageName = packageName;
            _playMode = playMode;
        }
        public async UniTask StartUpdate()
        {
            Debug.Log($"资源系统运行模式：{_playMode}");

            // 1. 全局初始化 YooAsset（只需一次）
            YooAssets.Initialize();

            // 2. 创建资源包
            var package = YooAssets.CreatePackage(_packageName);

            // 3. 根据运行模式构造不同的初始化参数
            InitializePackageOptions initOptions = null;

            switch (_playMode)
            {
                case EPlayMode.EditorSimulateMode:
#if UNITY_EDITOR
                    // 编辑器模拟模式（仅供开发调试）
                    var buildResult = EditorSimulateBuildInvoker.Build(_packageName, (int)EBundleType.VirtualAssetBundle);
                    var packageRoot = buildResult.PackageRootDirectory;
                    _fileSystemParams = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
                    initOptions = new EditorSimulateModeOptions
                    {
                        EditorFileSystemParameters = _fileSystemParams
                    };
#else
                    // 如果误在打包后使用模拟模式，强制转为离线模式
                    Debug.LogWarning("EditorSimulateMode 在非编辑器环境下无效，自动切换为 OfflinePlayMode");
                    _playMode = EPlayMode.OfflinePlayMode;
                    goto case EPlayMode.OfflinePlayMode;
#endif
                    break;

                case EPlayMode.OfflinePlayMode:
                    // 离线模式：直接从 StreamingAssets 加载
                    _fileSystemParams = FileSystemParameters.CreateDefaultBuiltinFileSystemParameters();

                    initOptions = new OfflinePlayModeOptions
                    {
                        BuiltinFileSystemParameters = _fileSystemParams
                    };
                    break;

                case EPlayMode.HostPlayMode:
                    // 如果将来需要远程更新，可在此扩展
                    // initOptions = new HostPlayModeOptions
                    // {
                    //     DefaultHostServer = "http://your-cdn.com/",
                    //     FallbackHostServer = "http://backup-cdn.com/"
                    // };
                    break;

                default:
                    throw new NotSupportedException($"不支持的运行模式：{_playMode}");
            }

            // 4. 初始化包
            var initOperation = package.InitializePackageAsync(initOptions);
            await initOperation;

            if (initOperation.Status == EOperationStatus.Succeeded)
            {
                Debug.Log("资源包初始化成功！");
            }
            else
            {
                Debug.LogError($"资源包初始化失败：{initOperation.Error}");
                _completionCallback?.Invoke(false);
                return;  // 初始化失败，直接停止后续步骤
            }

            // 5. 获取最新版本号（离线模式下通常使用构建时的固定版本）
            var reqVersionOp = package.RequestPackageVersionAsync();
            await reqVersionOp;
            if (reqVersionOp.Status != EOperationStatus.Succeeded)
            {
                Debug.LogError($"获取版本失败：{reqVersionOp.Error}");
                _completionCallback?.Invoke(false);
                return;
            }
            string packageVersion = reqVersionOp.PackageVersion;
            Debug.Log($"当前版本：{packageVersion}");

            // 6. 加载清单
            var loadManifestOp = package.LoadPackageManifestAsync(
                new LoadPackageManifestOptions(packageVersion, 60)
            );
            await loadManifestOp;

            if (loadManifestOp.Status == EOperationStatus.Succeeded)
            {
                Debug.Log("清单加载成功，更新流程完成！");
                _completionCallback?.Invoke(true);
            }
            else
            {
                Debug.LogError($"清单加载失败：{loadManifestOp.Error}");
                _completionCallback?.Invoke(false);
            }
        }
    }

}



