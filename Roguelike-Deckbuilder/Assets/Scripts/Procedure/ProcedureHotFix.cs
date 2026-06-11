using System.Collections;
using LitFramework.Asset;
using UnityEngine;
using YooAsset;
namespace LitFramework.FSM
{
    public class ProcedureHotFix : ProcedureBase
    {
        private StateMachine _machine;

        private IResourceUpdater _resourceUpdater;
        private bool _success = false;
        public ProcedureHotFix(StateMachine machine)
        {
            _machine = machine;
        }
        public override void OnInit()
        {
            _resourceUpdater = new YooAssetUpdater();
            Debug.Log($"初始化 HotFixState {_resourceUpdater == null}");
        }


        public override void OnEnter()
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
                ServiceLocator.Register<IAssetService>(new YooAssetAssetService(YooAssets.GetPackage("DefaultPackage")));
            }

        }
        public override void OnUpdate()
        {
            if (_success)
            {
                _machine.ChangeState<ProcedureMenu>();
            }
        }
        public override void OnExit()
        {

        }

        public override void OnDestroy()
        {

        }




    }
}