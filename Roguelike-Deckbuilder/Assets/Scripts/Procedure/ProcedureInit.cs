using System.Collections;
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
            ServiceLocator.Register(new ConfigService()); 
            ServiceLocator.Register(new UIService()); // 初始化对象池，预创建10个对象
            _isInitDone = true;
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