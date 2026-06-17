using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using LitFramework.Config;
using UnityEngine;
namespace LitFramework.FSM.Procedure

{
    public class ProcedureInitConfig : ProcedureBase
    {
        private StateMachine _machine;
        private const string CardConfigPath = "Assets/Config/Json/CardConfig.json";
        private const string CardEffectsPath = "Assets/Config/Json/CardEffects.json";
        private bool _isInitDone = false;
        private bool _isInitFailed = false;
        public ProcedureInitConfig(StateMachine machine)
        {
            _machine = machine;
        }
        public override void OnInit()
        {

        }


        public override void OnEnter()
        {
            _isInitDone = false;
            _isInitFailed = false;
            LoadAllConfigs().Forget();
        }
        public override void OnUpdate()
        {
            if (_isInitDone)
            {
                _machine.ChangeState<ProcedureTitle>();
            }
            else if (_isInitFailed)
            {
                Debug.LogError("ProcedureInitConfig失败");
            }
        }
        public override void OnExit()
        {

        }

        public override void OnDestroy()
        {

        }

        public async UniTask LoadAllConfigs()
        {
            // try
            // {
                var configSvc = ServiceLocator.Get<IConfigService>();
                await configSvc.LoadDictTableAsync<CardConfig>(CardConfigPath);
                await configSvc.LoadListTableAsync<CardEffects>(CardEffectsPath);
                ModelContainer.Register<ICardLibrary>(new CardLibrary());
                _isInitDone = true;
            // }
            // catch (Exception e)
            // {
            //     _isInitFailed = true;
            //     Debug.LogError($"ProcedureInitConfig失败:{e.Message}");
            // }

        }
    }
}