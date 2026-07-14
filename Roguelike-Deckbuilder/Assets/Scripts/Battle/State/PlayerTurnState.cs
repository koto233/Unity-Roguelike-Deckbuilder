using System.Collections;
using System.Collections.Generic;
using LitFramework.FSM;
using UnityEngine;

public class PlayerTurnState : TurnStateBase
{
    public PlayerTurnState(BattleController controller, StateMachine stateMachine) : base(controller, stateMachine)
    {
    }

    public override void OnInit()
    {

    }


    public override void OnEnter()
    {
        Debug.Log("玩家回合开始");
        // TODO: 玩家回合开始 抽卡，恢复能量 解锁 UI 交互
        Controller.StartPlayerTurn();
    }

    public override void OnExit()
    {
        // TODO: 玩家回合结束 锁定 UI 交互
        Controller.EndPlayerTurn();

    }


    public override void OnUpdate()
    {

    }
    public override void OnDestroy()
    {

    }
}

