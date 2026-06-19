using LitFramework.Asset;
using LitFramework.UI.Core.Service;
using UnityEngine;

namespace LitFramework.FSM.Procedure
{
    public class ProcedureTitle : ProcedureBase
    {
        public ProcedureTitle(ProcedureManager procedureManager) : base(procedureManager)
        {

        }
        public override void OnInit()
        {

        }
        public override void OnEnter()
        {
            // ServiceLocator.Get<IAssetService>().LoadAsync<GameObject>("Assets/Res/UI/UITitleWindow.prefab", (go) =>
            // {
            //     GameObject.Instantiate(go);
            // });
            ServiceLocator.Get<UIService>().OpenUI<UITitleWindow>();

        }
        public override void OnUpdate()
        {

        }
        public override void OnDestroy()
        {

        }


        public override void OnExit()
        {

        }



    }
}