using System.Collections;
using System.Collections.Generic;
using LitFramework.FSM;
using UnityEngine;

public class BattleEndState : TurnStateBase
{
    public BattleEndState(BattleController controller, StateMachine stateMachine) : base(controller, stateMachine)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("战斗结束");
    }

    public override void OnExit()
    {

    }

    public override void OnInit()
    {

    }

    public override void OnUpdate()
    {

    }
    public override void OnDestroy()
    {

    }

}
