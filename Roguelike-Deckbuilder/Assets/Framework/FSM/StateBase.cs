using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace LitFramework.FSM
{
    public class StateBase : IState
    {
        protected StateMachine StateMachine { get; }
        public StateBase(StateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }
        public virtual void OnInit() { }
        public virtual void OnEnter() { }
        public virtual void OnUpdate() { }
        public virtual void OnExit() { }
        public virtual void OnDestroy() { }
    }

}

