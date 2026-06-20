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
        private AssetRef<GameObject> cardPrefabRef;
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
            cardPrefabRef?.Dispose();
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
            cardPrefabRef = await assetService.LoadRefAsync<GameObject>("Assets/Res/UI/UICardItem.prefab");
            var battleContext = new BattleContext()
            {
                Player = new PlayerData(10, 3),
                Enemies = new List<EnemyData>()
                 {
                new EnemyData(20, 3),
                 },
                CurrentTurn = 0,
                IsPlayerTurn = true,
                Target = null,
                GoldReward = 0
            };
            var battleController = new BattleController(battleContext, () =>
            {
                uiService.Close<UIBattleWindow>();
                uiService.OpenUI<UITitleWindow>();
            });
            var uiBattleWindow = await uiService.OpenAsync<UIBattleWindow>();
            var uiBattlePresenter = new UIBattlePresenter(uiBattleWindow, battleController);
            uiBattleWindow.Init(cardPrefabRef.Asset);
            battleController.StartBattle();
            uiService.Close<UITitleWindow>();
        }
    }
}