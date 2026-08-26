using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LitFramework;
using LitFramework.UI.Core.Service;


public class RestPresenter : BasePresenter<RestView>
{
    private UIService _uiService;
    public RestPresenter(RestView view) : base(view)
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