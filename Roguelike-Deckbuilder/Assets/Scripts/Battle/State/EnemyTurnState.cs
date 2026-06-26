using System.Collections;
using System.Collections.Generic;
using LitFramework.FSM;
using UnityEngine;

public class EnemyTurnState : IState
{
    private BattleController _battleController;
    public EnemyTurnState(BattleController battleController)
    {
        _battleController = battleController;
    }

    public void OnEnter()
    {
        _battleController.StartEnemyTurn();
    }

    public void OnExit()
    {
        _battleController.EndEnemyTurn();
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
