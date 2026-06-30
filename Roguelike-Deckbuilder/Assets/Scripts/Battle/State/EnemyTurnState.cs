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
        EnemyAction().Forget();
    }
    private async UniTask EnemyAction()
    {
        foreach (var enemy in _controller.Context.Enemies)
        {
            if (enemy.CurrentHp <= 0) continue;

            // 1. 决策
            enemy.DetermineIntent(_controller.Context);

            // 2. 发布意图事件（UI更新）
            // EventBus.Publish(new EnemyIntentDeterminedEvent(enemy));

            // 3. 视觉延迟
            await UniTask.Delay(300);

            // 4. 执行
            enemy.ExecuteIntent(_controller.Context);

            // 5. 间隔
            await UniTask.Delay(500);
        }

        if (_controller.Context.Player.CurrentHp <= 0)
        {
            _controller.BattleFSM.ChangeState<BattleEndState>();
            return;
        }

        _controller.Context.Enemies.RemoveAll(e => e.CurrentHp <= 0);

        foreach (var enemy in _controller.Context.Enemies)
            enemy.OnTurnEnd();

        _controller.BattleFSM.ChangeState<PlayerTurnState>();
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
