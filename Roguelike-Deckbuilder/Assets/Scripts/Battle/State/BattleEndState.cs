using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.FSM;
using UnityEngine;

public class BattleEndState : TurnStateBase
{
    public BattleEndState(BattleController controller, StateMachine stateMachine) : base(controller, stateMachine)
    {
    }

    public override void OnEnter()
    {
        var globalPlayer = ServiceLocator.Get<PlayerDataService>();
        globalPlayer.SyncHp(Controller.Context.Player.CurrentHp, Controller.Context.Player.MaxHp);
        Controller.EndBattle();
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
