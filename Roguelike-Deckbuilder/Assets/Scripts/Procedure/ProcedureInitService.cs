using System.Collections;
using LitFramework.Audio;
using LitFramework.Config;
using LitFramework.ObjectPool;
using LitFramework.UI.Core.Service;
using UnityEngine;

namespace LitFramework.FSM.Procedure
{
    public class ProcedureInitService : ProcedureBase
    {

        private bool _isInitDone = false;
        public ProcedureInitService(ProcedureManager procedureManager) : base(procedureManager)
        {
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
            ServiceLocator.Register(new BattleInteractionService());
            ServiceLocator.Register<ISceneLoader>(new SceneLoader());
            ServiceLocator.Register(new MapService());
            ServiceLocator.Get<UIService>().Register<UITitleWindow>("Assets/Res/UI/UITitleWindow.prefab", UILayer.Normal);
            ModelContainer.Register(new PlayerModel());

            _isInitDone = true;
        }

        public override void OnUpdate()
        {
            if (_isInitDone)
            {
                _procedureManager.ChangeProcedure<ProcedureInitResource>();
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