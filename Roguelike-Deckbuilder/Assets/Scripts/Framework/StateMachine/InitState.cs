using System.Collections;
using Framework.ObjectPool;
using UnityEngine;

namespace Framework.State
{
    public class InitState : IState
    {
        private StateMachine _machine;
        private bool _isInitDone = false;
        public InitState(StateMachine machine)
        {
            _machine = machine;
        }

        public void OnInit()
        {

        }

        public void OnEnter()
        {
            // Debug.Log("进入InitState，进行框架初始化");
            _isInitDone = false;
            ServiceLocator.Register(new ObjectPoolService());
            _isInitDone = true;
        }

        public void OnUpdate()
        {
            if (_isInitDone)
            {
                _machine.ChangeState<HotFixState>();
            }
        }

        public void OnExit()
        {

        }

        public void OnDestroy()
        {

        }




    }
}