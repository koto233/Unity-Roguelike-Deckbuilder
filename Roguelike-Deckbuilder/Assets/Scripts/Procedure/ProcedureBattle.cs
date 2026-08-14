using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitFramework.Asset;
using LitFramework.Config;
using LitFramework.FSM;
using LitFramework.UI.Core.Service;
using UnityEngine;
namespace LitFramework.FSM.Procedure
{
    public class ProcedureBattle : ProcedureBase
    {
        public ProcedureBattle(ProcedureManager procedureManager) : base(procedureManager) { }
        private BattlePresenter _battlePresenter;
        private BattleController _battleController;
        private const string BattleSceneName = "Battle Scene"; // 与场景文件名称一致
        public override void OnInit()
        {

        }

        public override void OnEnter()
        {
            if (!_procedureManager.TryGetArgs<BattleStartParams>(out var args))
            {
                Debug.LogError("缺少战斗参数，无法初始化");
                return;
            }
            InitBattleAsync(args).Forget();
        }

        public override void OnExit()
        {
            CleanupBattleAsync().Forget();
            _battlePresenter?.Dispose();
            _battlePresenter = null;
            _battleController = null;
        }


        public override void OnUpdate()
        {
        }
        public override void OnDestroy()
        {

        }
        private async UniTaskVoid InitBattleAsync(BattleStartParams args)
        {
            var sceneLoader = ServiceLocator.Get<ISceneLoader>();
            var assetService = ServiceLocator.Get<IAssetService>();
            var uiService = ServiceLocator.Get<UIService>();
            var configService = ServiceLocator.Get<IConfigService>();
            var cardIconService = ServiceLocator.Get<CardIconService>();
            await cardIconService.PreLoadCardIcons();
            var battleContext = new BattleContext()
            {
                Player = new Player(100, 3),
                Enemies = new(),
                CurrentTurn = 0,
                IsPlayerTurn = true,
                Target = null,
                GoldReward = 0
            };
            foreach (var enemyId in args.EnemyIds)
            {
                var enemyConfig = configService.GetTable<EnemyConfig>().Get(enemyId);
                var enemyAi = EnemyAIFactory.Create(enemyConfig.AIType);
                battleContext.Enemies.Add(new Enemy(enemyConfig, enemyAi));
            }

            // var uiBattleWindow = await uiService.OpenAsync<UIBattleWindow>(battleContext);
            var scene = await sceneLoader.LoadAdditiveAsync(BattleSceneName);

            _battleController = new BattleController(battleContext, OnBattleEnd);
            BattleSceneRoot battleCtx = null;
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.TryGetComponent(out battleCtx))
                    break;
            }
            var uiBattleWindow = battleCtx.UIBattle;
            if (uiBattleWindow != null)
                await uiBattleWindow.InitAsync(battleContext);
            _battlePresenter = new BattlePresenter(uiBattleWindow, _battleController);

            _battleController.StartBattle();
            uiService.Close<UITitleWindow>();
        }
        private async UniTaskVoid CleanupBattleAsync()
        {
            // 释放 Presenter
            _battlePresenter?.Dispose();
            _battlePresenter = null;
            _battleController = null;

            // 卸载战斗场景
            var sceneLoader = ServiceLocator.Get<ISceneLoader>();
            if (sceneLoader.IsSceneLoaded(BattleSceneName))
            {
                await sceneLoader.UnloadAdditiveAsync(BattleSceneName);
            }
        }

        private void OnBattleEnd()
        {
            // 切换到地图流程
            _procedureManager.ChangeProcedure<ProcedureMap>();
        }
    }
}