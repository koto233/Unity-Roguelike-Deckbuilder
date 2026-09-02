using Cysharp.Threading.Tasks;
using LitFramework.Asset;
using LitFramework.UI.Core.Service;
using UnityEngine;

namespace LitFramework.FSM.Procedure
{
    public class ProcedureTitle : ProcedureBase
    {
        public override string Name => "Title";
        public ProcedureTitle(ProcedureManager procedureManager) : base(procedureManager)
        {

        }
        public override void OnInit()
        {

        }
        public override void OnEnter()
        {
            ServiceLocator.Get<UIService>().OpenAsync<MainMenuView>().Forget();
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