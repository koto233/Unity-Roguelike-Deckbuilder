using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LitFramework.FSM;


public abstract class TurnStateBase : StateBase
{
    protected BattleController Controller { get; }
    protected TurnStateBase(BattleController controller, StateMachine stateMachine) : base(stateMachine)
    {
        Controller = controller;
    }

}
public enum TurnResult
{
    Continue,
    PlayerWin,
    PlayerLose,
    GameOver
}