using System.Collections;
using System.Collections.Generic;
using LitFramework.FSM;
using UnityEngine;

public class PlayerTurnState : IState
{
    private BattleController _battleController;
    public PlayerTurnState(BattleController battleController)
    {
        _battleController = battleController;
    }
    public void OnInit()
    {

    }


    public void OnEnter()
    {
        Debug.Log("玩家回合开始");
        // TODO: 玩家回合开始 抽卡，恢复能量 解锁 UI 交互
        _battleController.StartPlayerTurn();
    }

    public void OnExit()
    {
        // TODO: 玩家回合结束 锁定 UI 交互
        _battleController.EndPlayerTurn();
       
    }


    public void OnUpdate()
    {

    }
    public void OnDestroy()
    {

    }
}

public enum TurnResult
{
    Continue,
    PlayerWin,
    PlayerLose,
    GameOver
}