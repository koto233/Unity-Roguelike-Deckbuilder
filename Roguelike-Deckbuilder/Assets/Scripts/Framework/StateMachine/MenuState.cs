using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Framework.State
{
    public class MenuState : IState
    {
        public StateMachine _machine;
        public MenuState(StateMachine machine)
        {
            _machine = machine;
        }
        public void OnInit()
        {

        }
        public void OnEnter()
        {

        }
        public void OnUpdate()
        {

        }
        public void OnDestroy()
        {

        }


        public void OnExit()
        {

        }



    }
}