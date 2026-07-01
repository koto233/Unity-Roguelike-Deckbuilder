using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework.FSM;
using UnityEngine;

public class EnemyTurnState : IState
{
    private BattleController _controller;
    public EnemyTurnState(BattleController battleController)
    {
        _controller = battleController;
    }

    public void OnEnter()
    {
        Debug.Log("敌人回合开始");
        // _controller.StartEnemyTurn();
        ExecuteEnemyTurn().Forget();
    }
    private async UniTask ExecuteEnemyTurn()
    {
        // State 不知道 Controller 内部怎么判断胜负
        // State 只拿到一个结果
        TurnResult result = await _controller.ExecuteEnemyTurnAsync();

        // 根据结果决定跳转
        switch (result)
        {
            case TurnResult.PlayerWin:
                _controller.BattleFSM.ChangeState<BattleEndState>();
                break;
            case TurnResult.PlayerLose:
                _controller.BattleFSM.ChangeState<BattleEndState>();
                break;
            case TurnResult.Continue:
                _controller.BattleFSM.ChangeState<PlayerTurnState>();
                break;
        }
    }
    public void OnExit()
    {
        // _controller.EndEnemyTurn();
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
