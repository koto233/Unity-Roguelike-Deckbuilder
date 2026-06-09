using System.Collections;
using Framework.ObjectPool;
using UnityEngine;

namespace Framework.State
{
    public class BootState : ICoroutineState
    {
        private StateMachine _machine;
        public BootState(StateMachine machine)
        {
            _machine = machine;
        }

        public void OnInit()
        {

        }

        public void OnEnter()
        {
            // Debug.Log("进入BootState，进行框架初始化");


        }

        public IEnumerator OnEnterCoroutine()
        {
            ServiceLocator.Register(new ObjectPoolService());
            yield return null;
            _machine.ChangeStateCoroutine<HitFixState>();
        }

        public void OnExit()
        {

        }

        public void OnDestroy()
        {

        }

        public void OnUpdate()
        {

        }


    }
}