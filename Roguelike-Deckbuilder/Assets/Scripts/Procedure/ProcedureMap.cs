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
        private MapPresenter _mapPresenter;
        private const string MapSceneName = "Map Scene";

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
            _mapPresenter = null;
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
            EventBus<RestStartEvent>.Subscribe(OnRestStart);
            EventBus<ShopOpenEvent>.Subscribe(OnShopOpen);
            EventBus<EventStartEvent>.Subscribe(OnEventStart);
        }

        private void UnsubscribeEventHandlers()
        {
            EventBus<BattleStartEvent>.Unsubscribe(OnBattleStart);
            EventBus<RestStartEvent>.Unsubscribe(OnRestStart);
            EventBus<ShopOpenEvent>.Unsubscribe(OnShopOpen);
            EventBus<EventStartEvent>.Unsubscribe(OnEventStart);
        }

        // ============ 事件处理器 ============
        private void OnBattleStart(BattleStartEvent evt)
        {

            var args = new BattleStartParams { EnemyIds = evt.EnemyIds };
            _procedureManager.ChangeProcedure<ProcedureBattle, BattleStartParams>(args);
        }

        private void OnRestStart(RestStartEvent evt)
        {
            Debug.Log("进入休息点");
            var uiService = ServiceLocator.Get<UIService>();
            // uiService.OpenAsync<UIRestWindow>(); // 假设你有休息窗口
        }

        private void OnShopOpen(ShopOpenEvent evt)
        {
            Debug.Log("打开商店");
            var uiService = ServiceLocator.Get<UIService>();
            // uiService.OpenAsync<UIShopWindow>();
        }

        private void OnEventStart(EventStartEvent evt)
        {
            Debug.Log("触发随机事件");
            var uiService = ServiceLocator.Get<UIService>();
            // uiService.OpenAsync<UIEventWindow>();
        }

        // ============ 地图初始化 ============
        private async UniTaskVoid InitAsync()
        {
            var sceneLoader = ServiceLocator.Get<ISceneLoader>();
            var uiService = ServiceLocator.Get<UIService>();
            await sceneLoader.LoadAdditiveAsync(MapSceneName);
            var topBar = await uiService.OpenAsync<UITopBar, TopBarPresenter>();
            var uiMap = GameObject.FindObjectOfType<UIMap>(true);
            if (uiMap != null)
            {
                await uiMap.InitAsync();
                _mapPresenter = new MapPresenter(uiMap);
                _mapPresenter.CreateMapUI();
            }
            else
            {
                Debug.LogError("UIMap 组件未找到");
            }
            uiService.Close<UITitleWindow>();
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