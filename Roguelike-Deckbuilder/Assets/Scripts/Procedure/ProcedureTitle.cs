using Cysharp.Threading.Tasks;
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
            ServiceLocator.Get<UIService>().OpenAsync<UITitleWindow, TitlePresenter>().Forget();
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