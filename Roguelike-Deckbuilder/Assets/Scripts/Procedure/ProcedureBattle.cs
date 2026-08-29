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
        private BattleController _battleController;
        private const string BattleSceneName = "Battle Scene"; // 与场景文件名称一致
        public override void OnInit()
        {
            _battleController = ServiceLocator.Get<BattleController>();
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
        }


        public override void OnUpdate()
        {
        }
        public override void OnDestroy()
        {

        }
        private async UniTaskVoid InitBattleAsync(BattleStartParams args)
        {
            // var sceneLoader = ServiceLocator.Get<ISceneLoader>();
            var uiService = ServiceLocator.Get<UIService>();
            // var scene = await sceneLoader.LoadAdditiveAsync(BattleSceneName);
            // BattleSceneRoot battleCtx = null;
            // foreach (var root in scene.GetRootGameObjects())
            // {
            //     if (root.TryGetComponent(out battleCtx))
            //         break;
            // }
            // _battlePresenter = new BattlePresenter(battleCtx.UIBattle);
            // _battlePresenter.Init(battleContext);
            _battleController.StartBattle(args);
            await uiService.OpenAsync<BattleView, BattleContext>(_battleController.Context);
            uiService.Close<MainMenuView>();
        }
        private async UniTaskVoid CleanupBattleAsync()
        {
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