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
        bool _initing = false;
        public ProcedureBattle(ProcedureManager procedureManager) : base(procedureManager) { }

        public override void OnInit()
        {

        }

        public override void OnEnter()
        {
            if (_initing) return;
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
            _initing = true;
            var assetService = ServiceLocator.Get<IAssetService>();
            var uiService = ServiceLocator.Get<UIService>();
            var cardPrefab = await assetService.LoadAsync<GameObject>("Assets/Res/UI/UICardItem.prefab");
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
            uiBattleWindow.Init(cardPrefab);
            battleController.StartBattle();
            uiService.Close<UITitleWindow>();
            _initing = false;
        }
    }
}