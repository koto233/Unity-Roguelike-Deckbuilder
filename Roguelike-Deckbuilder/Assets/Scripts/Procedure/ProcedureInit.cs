using System.Collections;
using LitFramework.Audio;
using LitFramework.Config;
using LitFramework.ObjectPool;
using LitFramework.UI.Core.Service;
using UnityEngine;

namespace LitFramework.FSM
{
    public class ProcedureInit : ProcedureBase
    {
        private StateMachine _machine;
        private bool _isInitDone = false;
        public ProcedureInit(StateMachine machine)
        {
            _machine = machine;
        }

        public override void OnInit()
        {

        }

        public override void OnEnter()
        {
            // Debug.Log("进入InitState，进行框架初始化");
            _isInitDone = false;
            ServiceLocator.Register(new ObjectPoolService());
            ServiceLocator.Register<IConfigService>(new ConfigService());
            ServiceLocator.Register(new InputService());
            ServiceLocator.Register<IAudioService>(new AudioService());
            ServiceLocator.Register(new UIService());
            ServiceLocator.Get<UIService>().Register<UITitleWindow>("Assets/Res/UI/UITitleWindow.prefab", UILayer.Normal);
            ServiceLocator.Get<UIService>().Register<UIBattleWindow>("Assets/Res/UI/UIBattleWindow.prefab", UILayer.Normal);
            LoadAllConfigs();
            _isInitDone = true;
        }
        // 在 GameRoot 或启动时加载
        public void LoadAllConfigs()
        {
            var configSvc = ServiceLocator.Get<IConfigService>();
            configSvc.LoadTable<CardConfig>("Cards", (success) =>
            {
                if (success)
                {
                    ModelContainer.Register<ICardLibrary>(new CardLibrary());
                }
                else
                {
                    Debug.LogError("卡牌配置加载失败");
                }
            });

            ModelContainer.Register(new PlayerModel());
        }
        public override void OnUpdate()
        {
            if (_isInitDone)
            {
                _machine.ChangeState<ProcedureHotFix>();
            }
        }

        public override void OnExit()
        {

        }

        public override void OnDestroy()
        {

        }




    }
}