using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.UI.Core.Service;
using UnityEngine;

public class ShopPresenter : BasePresenter<ShopView>
{
    private UIService _uiService;
    public ShopPresenter(ShopView view) : base(view)
    {
    }

    public override void Init()
    {
        _uiService = ServiceLocator.Get<UIService>();
        SubscribeEvents();
    }
    private void SubscribeEvents()
    {
        View.OnClickContinue += HandleClickContinue;
    }



    private void UnsubscribeEvents()
    {
        View.OnClickContinue -= HandleClickContinue;
    }
    public override void Dispose()
    {
        base.Dispose();
        UnsubscribeEvents();
    }

    private void HandleClickContinue()
    {
        _uiService.Close<ShopView>();
    }
}
