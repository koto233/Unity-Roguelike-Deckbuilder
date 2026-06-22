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
        private const string CardConfigPath = "Assets/Config/Json/CardConfig.json";
        private const string CardEffectsPath = "Assets/Config/Json/CardEffects.json";
        public ProcedureInitConfig(ProcedureManager procedureManager) : base(procedureManager) { }

        public override void OnInit()
        {

        }


        public override void OnEnter()
        {
            LoadAllConfigs().Forget();
        }
        public override void OnUpdate()
        {

        }
        public override void OnExit()
        {

        }

        public override void OnDestroy()
        {

        }

        public async UniTask LoadAllConfigs()
        {
            var configSvc = ServiceLocator.Get<IConfigService>();
            await configSvc.LoadDictTableAsync<CardConfig>(CardConfigPath);
            await configSvc.LoadListTableAsync<CardEffectsConfig>(CardEffectsPath);
            ModelContainer.Register<ICardLibrary>(new CardLibrary());
            _procedureManager.ChangeProcedure<ProcedureTitle>();
        }
    }
}