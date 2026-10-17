using System;
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
        public override string Name => "Battle";

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
            var uiService = ServiceLocator.Get<UIService>();
            try
            {
                // 1. 只创建战斗数据，不启动状态机
                _battleController.CreateBattle(args);

                // 2. 打开战斗界面。OpenAsync 内部会 await BattlePresenter.Init()，
                //    返回时玩家/敌人/意图图标等子视图资源已经全部就绪
                await uiService.OpenAsync<BattleView, BattleContext>(_battleController.Context);
                uiService.Close<MainMenuView>();

                // 3. UI 就绪后再启动状态机，第一次玩家回合才能正确显示敌人意图
                _battleController.BeginBattle();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
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