using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.FSM.Procedure;
using LitFramework.UI.Core.Service;
using UnityEngine;

public class BattleResultPresenter : BasePresenter<BattleResultPanel>
{

    public BattleResultPresenter(BattleResultPanel view) : base(view)
    {

    }
    public override void Init()
    {
        SubscribeEvents();
    }
    private void SubscribeEvents()
    {
        View.OnSkipClick += HandleSkipClick;
    }
    private void UnsubscribeEvents()
    {
        View.OnSkipClick -= HandleSkipClick;
    }

    private void HandleSkipClick()
    {
        ServiceLocator.Get<UIService>().Close<BattleResultPanel>();
        ServiceLocator.Get<ProcedureManager>().ChangeProcedure<ProcedureMap>();
    }

    public override void Dispose()
    {
        UnsubscribeEvents();
    }


}
