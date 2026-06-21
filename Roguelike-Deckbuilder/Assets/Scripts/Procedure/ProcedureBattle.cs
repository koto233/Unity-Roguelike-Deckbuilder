using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitFramework.Asset;
using LitFramework.FSM;
using LitFramework.UI.Core.Service;
using UnityEngine;
namespace LitFramework.FSM.Procedure
{
    public class ProcedureBattle : ProcedureBase
    {
        public ProcedureBattle(ProcedureManager procedureManager) : base(procedureManager) { }

        public override void OnInit()
        {

        }

        public override void OnEnter()
        {
            InitBattleAsync().Forget();
        }

        public override void OnExit()
        {

        }


        public override void OnUpdate()
        {
        }
        public override void OnDestroy()
        {

        }
        private async UniTaskVoid InitBattleAsync()
        {
            var assetService = ServiceLocator.Get<IAssetService>();
            var uiService = ServiceLocator.Get<UIService>();

            var battleContext = new BattleContext()
            {
                Player = new PlayerData(10, 3),
                Enemies = new List<EnemyData>()
                 {
                new(20, 3),
                 },
                CurrentTurn = 0,
                IsPlayerTurn = true,
                Target = null,
                GoldReward = 0
            };
            var battleController = new BattleController(battleContext, () =>
            {
                uiService.Close<UIBattleWindow>();
                uiService.OpenAsync<UITitleWindow>().Forget();
            });
            var uiBattleWindow = await uiService.OpenAsync<UIBattleWindow>(battleContext);
            var uiBattlePresenter = new UIBattlePresenter(uiBattleWindow, battleController);
            battleController.StartBattle();
            uiService.Close<UITitleWindow>();
        }
    }
}