using System.Collections;
using Cysharp.Threading.Tasks;
using LitFramework.Asset;
using LitFramework.UI.Core.Service;
using UnityEngine;
using YooAsset;
namespace LitFramework.FSM.Procedure
{
    public class ProcedureInitResource : ProcedureBase
    {
        private IResourceUpdater _resourceUpdater;
        public ProcedureInitResource(ProcedureManager procedureManager) : base(procedureManager) { }

        public override void OnInit()
        {
            _resourceUpdater = new YooAssetUpdater();
            Debug.Log($"初始化 HotFixState {_resourceUpdater == null}");
        }


        public override void OnEnter()
        {
            InitResAsync().Forget();
        }

        private async UniTask InitResAsync()
        {
            await _resourceUpdater.StartUpdate();
            ServiceLocator.Register<IAssetService>(new YooAssetAssetService(YooAssets.GetPackage("DefaultPackage")));
            _procedureManager.ChangeProcedure<ProcedureInitConfig>();
        }
        public override void OnExit()
        {

        }

        public override void OnDestroy()
        {

        }

        public override void OnUpdate()
        {
           
        }
    }
}