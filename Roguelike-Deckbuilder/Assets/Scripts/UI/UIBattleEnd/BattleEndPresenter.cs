using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.FSM.Procedure;
using LitFramework.UI.Core.Service;
using UnityEngine;

public class BattleEndPresenter : BasePresenter<UIBattleEnd>
{

    public BattleEndPresenter(UIBattleEnd view) : base(view)
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
        ServiceLocator.Get<UIService>().Close<UIBattleEnd>();
        ServiceLocator.Get<ProcedureManager>().ChangeProcedure<ProcedureMap>();
    }

    public override void Dispose()
    {
        UnsubscribeEvents();
    }


}
