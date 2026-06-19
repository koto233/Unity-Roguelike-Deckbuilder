using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework.Asset;
using LitFramework.FSM;
using LitFramework.UI.Core.Service;
using UnityEngine;
namespace LitFramework.FSM.Procedure
{
    public class ProcedureBattle : ProcedureBase
    {
        private UIService _uiService;
        // private bool _isInitDone = false;
        public ProcedureBattle(ProcedureManager procedureManager) : base(procedureManager) { }

        public UIService UIService
        {
            get
            {
                if (_uiService == null)
                {
                    _uiService = ServiceLocator.Get<UIService>();
                }
                return _uiService;
            }
        }
        public override void OnInit()
        {

        }

        public override void OnEnter()
        {
            StartBattleAsync().Forget();
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
        private async UniTaskVoid StartBattleAsync()
        {
            var assetService = ServiceLocator.Get<IAssetService>();
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
                UIService.Close<UIBattleWindow>();
                UIService.OpenUI<UITitleWindow>();
            });
            var uiBattleWindow = await UIService.OpenAsync<UIBattleWindow>();
            var uiBattlePresenter = new UIBattlePresenter(uiBattleWindow, battleController);
            uiBattleWindow.Init(cardPrefab, battleContext);
            battleController.StartBattle();
            UIService.Close<UITitleWindow>();
            // _isInitDone = true;
        }
    }
}