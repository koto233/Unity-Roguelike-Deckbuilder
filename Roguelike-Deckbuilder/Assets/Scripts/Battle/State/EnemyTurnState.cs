using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.FSM;
using LitFramework.UI.Core.Service;
using UnityEngine;

public class EnemyTurnState : TurnStateBase
{
    public EnemyTurnState(BattleController controller, StateMachine stateMachine) : base(controller, stateMachine)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("敌人回合开始");
        // _controller.StartEnemyTurn();
        ExecuteEnemyTurn().Forget();
    }
    private async UniTask ExecuteEnemyTurn()
    {
        // State 不知道 Controller 内部怎么判断胜负
        // State 只拿到一个结果
        TurnResult result = await Controller.ExecuteEnemyTurnAsync();
        // 根据结果决定跳转
        switch (result)
        {
            case TurnResult.PlayerWin:
                StateMachine.ChangeState<BattleEndState>();
                break;
            // case TurnResult.PlayerLose:
            //     ServiceLocator.Get<UIService>().OpenAsync<GameOverView>().Forget();
                // break;
            case TurnResult.Continue:
                StateMachine.ChangeState<PlayerTurnState>();
                break;
        }
    }
    public override void OnExit()
    {
        // _controller.EndEnemyTurn();
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
