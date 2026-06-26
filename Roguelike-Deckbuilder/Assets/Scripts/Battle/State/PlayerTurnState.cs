using System.Collections;
using System.Collections.Generic;
using LitFramework.FSM;
using UnityEngine;

public class PlayerTurnState : IState
{
    private StateMachine _machine;
    public PlayerTurnState(StateMachine machine)
    {
        _machine = machine;
    }


    public void OnEnter()
    {
        // TODO: 玩家回合开始 抽卡，恢复能量 解锁 UI 交互
    }

    public void OnExit()
    {

    }

    public void OnInit()
    {

    }

    public void OnUpdate()
    {

    }
    public void OnDestroy()
    {

    }
}
