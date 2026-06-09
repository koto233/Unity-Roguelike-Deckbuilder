using System.Collections;
using Framework.AssetManager;
using UnityEngine;
using YooAsset;
namespace Framework.State
{
    public class HitFixState : ICoroutineState
    {
        private StateMachine _machine;

        private IResourceUpdater _resourceUpdater;
        private bool _completed = false;
        private bool _success = false;
        public HitFixState(StateMachine machine)
        {
            _machine = machine;
        }
        public void OnInit()
        {
            _resourceUpdater = new YooAssetUpdater();
            Debug.Log($"初始化 HitFixState {_resourceUpdater == null}");
        }


        public void OnEnter()
        {
            // Debug.Log($"进入 HitFixState，开始资源更新流程{_resourceUpdater == null}");

            _resourceUpdater.StartUpdate(OnResourceUpdateCompleted);
        }

        public IEnumerator OnEnterCoroutine()
        {
            yield return new WaitUntil(() => _completed);
            if (!_success)
            {
                Debug.LogError("热更新失败，无法继续");
                // 可以进入错误处理状态或显示 UI
            }
        }

        public void OnExit()
        {

        }

        public void OnDestroy()
        {

        }

        public void OnUpdate()
        {

        }

        private void OnResourceUpdateCompleted(bool flag)
        {
            _completed = true;
            _success = flag;
            if (flag)
            {
                Debug.Log("资源更新完成，开始游戏逻辑");
                ServiceLocator.Register<IAssetManager>(new YooAssetAssetManager(YooAssets.GetPackage("DefaultPackage")));
            }

        }

    }
}