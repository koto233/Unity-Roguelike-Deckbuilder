using System.Collections;
using Framework.AssetManager;
using UnityEngine;
using YooAsset;
namespace Framework.State
{
    public class HotFixState : IState
    {
        private StateMachine _machine;

        private IResourceUpdater _resourceUpdater;
        private bool _success = false;
        public HotFixState(StateMachine machine)
        {
            _machine = machine;
        }
        public void OnInit()
        {
            _resourceUpdater = new YooAssetUpdater();
            Debug.Log($"初始化 HotFixState {_resourceUpdater == null}");
        }


        public void OnEnter()
        {
            // Debug.Log($"进入 HitFixState，开始资源更新流程{_resourceUpdater == null}");
            _success = false;
            _resourceUpdater.StartUpdate(OnResourceUpdateCompleted);
        }
        private void OnResourceUpdateCompleted(bool flag)
        {
            _success = flag;
            if (flag)
            {
                Debug.Log("资源更新完成，开始游戏逻辑");
                ServiceLocator.Register<IAssetManager>(new YooAssetAssetManager(YooAssets.GetPackage("DefaultPackage")));
            }

        }
        public void OnUpdate()
        {
            if (_success)
            {
                _machine.ChangeState<MenuState>();
            }
        }
        public void OnExit()
        {

        }

        public void OnDestroy()
        {

        }




    }
}