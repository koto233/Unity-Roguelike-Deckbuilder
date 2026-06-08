using System;
using System.Collections;
using System.Collections.Generic;
using Framework.AssetManager;
using Framework.ObjectPool;
using UnityEngine;
using YooAsset;
namespace Framework
{
    /// <summary>
    /// 游戏根对象，负责管理全局服务和游戏状态
    /// </summary>
    public class GameRoot : MonoBehaviour
    {
        public static GameRoot Instance { get; private set; }

        private ServiceLocator _serviceLocator;
        private IResourceUpdater _resourceUpdater;
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }

        void Start()
        {

        }
        public void Test()
        {
            var assetManager = _serviceLocator.Get<IAssetManager>();
            assetManager.LoadAsync<GameObject>("Assets/Res/Test.prefab", (prefab) =>
            {
                var uiRoot = Instantiate(prefab);
                uiRoot.transform.SetParent(transform);
            });
        }
        private void Init()
        {
            _serviceLocator = new ServiceLocator();
            _serviceLocator.Register(new ObjectPoolService());
            _resourceUpdater = new YooAssetUpdater();
            _resourceUpdater.StartUpdate(OnResourceUpdateCompleted);

        }

        private void OnResourceUpdateCompleted(bool flag)
        {
            if (flag)
            {
                Debug.Log("资源更新完成，开始游戏逻辑");
                _serviceLocator.Register<IAssetManager>(new YooAssetAssetManager(YooAssets.GetPackage("DefaultPackage")));
            }

        }
    }

}
