using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using LitFramework.Asset;
using LitFramework.Config;
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
        }

        public override void OnExit()
        {
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

        private async UniTaskVoid InitAsync()
        {
            var sceneLoader = ServiceLocator.Get<ISceneLoader>();
            var uiService = ServiceLocator.Get<UIService>();
            await sceneLoader.LoadAdditiveAsync(MapSceneName);
            var uiMap = GameObject.FindObjectOfType<UIMap>(true);
            if (uiMap != null)
            {
                await uiMap.InitAsync();
                _mapPresenter = new MapPresenter(uiMap);
                _mapPresenter.GenerateMap(1);
            }
            else
            {
                Debug.LogError("UIMap 组件未找到");
            }

            uiService.Close<UITitleWindow>();
        }
    }
}