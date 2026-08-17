using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.FSM.Procedure;
using LitFramework.UI.Core.Service;
using UnityEngine;

public class BattleEndPresenter : BasePresenter
{
    private UIBattleEnd _view;
    public BattleEndPresenter(UIBattleEnd view)
    {
        _view = view;
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        _view.OnSkipClick += HandleSkipClick;
    }
    private void UnsubscribeEvents()
    {
        _view.OnSkipClick -= HandleSkipClick;
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
