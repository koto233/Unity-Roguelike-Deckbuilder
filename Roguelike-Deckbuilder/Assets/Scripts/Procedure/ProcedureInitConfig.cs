using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            LoadAllConfigs();
        }
        public override void OnUpdate()
        {
            if (_isInitDone)
            {
                _machine.ChangeState<ProcedureTitle>();
            }
        }
        public override void OnExit()
        {

        }

        public override void OnDestroy()
        {

        }

        public void LoadAllConfigs()
        {
            var configSvc = ServiceLocator.Get<IConfigService>();
            configSvc.LoadTable<CardConfig>(CardConfigPath, (success) =>
            {
                _isInitDone = success;
                if (success)
                {
                    Debug.Log("卡牌配置加载成功");
                    ModelContainer.Register<ICardLibrary>(new CardLibrary());
                }
                else
                {
                    Debug.LogError("卡牌配置加载失败");
                }
            });
            configSvc.LoadTable<CardConfig>(CardEffectsPath, (success) =>
            {
                _isInitDone = success;
                if (success)
                {
                    Debug.Log("效果配置加载成功");
                    ModelContainer.Register<ICardLibrary>(new CardLibrary());
                }
                else
                {
                    Debug.LogError("效果配置加载失败");
                }
            });
        }
    }
}