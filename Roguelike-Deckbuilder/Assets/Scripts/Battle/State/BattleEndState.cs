using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.FSM;
using LitFramework.UI.Core.Service;
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
        var uiService = ServiceLocator.Get<UIService>();
        uiService.Close<BattleView>();
        uiService.OpenAsync<BattleResultPanel>().Forget();
        //    Controller.EndBattle(); // 改为由UIBattleEnd上的按钮调用
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
