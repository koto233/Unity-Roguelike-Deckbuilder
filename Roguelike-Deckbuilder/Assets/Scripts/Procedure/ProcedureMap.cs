using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using LitFramework.Asset;
using LitFramework.Config;
using LitFramework.EventBus;
using LitFramework.UI.Core.Service;
using UnityEngine;

namespace LitFramework.FSM.Procedure
{
    public class ProcedureMap : ProcedureBase
    {
        public override string Name => "Map";
        private const string MapSceneName = "Map Scene";
        private UIService _uiService;
        public ProcedureMap(ProcedureManager procedureManager) : base(procedureManager)
        {
        }

        public override void OnEnter()
        {
            InitAsync().Forget();
            // ★ 订阅地图节点触发的事件
            SubscribeEventHandlers();
        }

        public override void OnExit()
        {

            UnloadMapSceneAsync().Forget();
            // ★ 取消订阅
            UnsubscribeEventHandlers();
            _uiService.Close<MapView>();
        }

        public override void OnInit()
        {
        }

        public override void OnUpdate()
        {
        }

        public override void OnDestroy()
        {
        }

        // ============ 事件订阅 ============
        private void SubscribeEventHandlers()
        {
            EventBus<BattleStartEvent>.Subscribe(OnBattleStart);

        }

        private void UnsubscribeEventHandlers()
        {
            EventBus<BattleStartEvent>.Unsubscribe(OnBattleStart);
        }

        // ============ 事件处理器 ============
        private void OnBattleStart(BattleStartEvent evt)
        {
            var args = new BattleStartParams { Type = evt.Type };
            _procedureManager.ChangeProcedure<ProcedureBattle, BattleStartParams>(args);
        }



        // ============ 地图初始化 ============
        private async UniTaskVoid InitAsync()
        {
            // var sceneLoader = ServiceLocator.Get<ISceneLoader>();
            _uiService = ServiceLocator.Get<UIService>();
            var uiatlasService = ServiceLocator.Get<UIAtlasService>();
            await uiatlasService.PreLoadCardIcons();
            // await sceneLoader.LoadAdditiveAsync(MapSceneName);
            await _uiService.OpenAsync<MapView>();
         
            // var uiMap = GameObject.FindObjectOfType<UIMap>(true);
            // if (uiMap != null)
            // {
            //     await uiMap.InitAsync();
            //     _mapPresenter = new MapPresenter(uiMap);
            //     _mapPresenter.CreateMapUI();
            // }
            // else
            // {
            //     Debug.LogError("UIMap 组件未找到");
            // }
            _uiService.Close<MainMenuView>();
        }

        private async UniTaskVoid UnloadMapSceneAsync()
        {
            var sceneLoader = ServiceLocator.Get<ISceneLoader>();
            if (sceneLoader.IsSceneLoaded(MapSceneName))
            {
                await sceneLoader.UnloadAdditiveAsync(MapSceneName);
                Debug.Log("地图场景已卸载");
            }
        }
    }
}