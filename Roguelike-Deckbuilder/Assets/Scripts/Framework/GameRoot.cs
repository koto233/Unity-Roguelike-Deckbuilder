using System;
using System.Collections;
using System.Collections.Generic;
using Framework.AssetManager;
using Framework.ObjectPool;
using Framework.State;
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
        private StateMachine _flowMachine;
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
            var assetManager = ServiceLocator.Get<IAssetManager>();
            assetManager.LoadAsync<GameObject>("Assets/Res/Test.prefab", (prefab) =>
            {
                var uiRoot = Instantiate(prefab);
                uiRoot.transform.SetParent(transform);
            });
        }
        private void Init()
        {
            _flowMachine = new StateMachine();
            _flowMachine.RegisterState(new BootState(_flowMachine));
            _flowMachine.RegisterState(new HitFixState(_flowMachine));
            // 监听状态变化（可选）
            _flowMachine.OnStateChanged += (from, to) =>
            {
                Debug.Log($"流程状态变化: {from?.Name} → {to?.Name}");
            };
            _flowMachine.ChangeStateCoroutine<BootState>();
        }


    }

}
