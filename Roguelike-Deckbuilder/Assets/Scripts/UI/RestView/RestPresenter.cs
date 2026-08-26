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
        View.CloseForge();
    }
    private void SubscribeEvents()
    {
        View.OnClickContinue += HandleClickContinue;
        View.OnClickForge += HandleClickForge;
        View.OnClickRest += HandleClickRest;
        View.OnClickCloseForge += HandleClickCloseForge;
    }


    private void UnsubscribeEvents()
    {
        View.OnClickContinue -= HandleClickContinue;
        View.OnClickForge -= HandleClickForge;
        View.OnClickRest -= HandleClickRest;
        View.OnClickCloseForge -= HandleClickCloseForge;
    }

    private void HandleClickRest()
    {
        var globalPlayer = ServiceLocator.Get<PlayerDataService>();
        globalPlayer.CurrentHp += (int)(globalPlayer.MaxHp * .3f);
        View.ShowContinue();
    }

    private void HandleClickForge()
    {
        View.OpenForge();
    }



    private void HandleClickContinue()
    {
        _uiService.Close<ShopView>();
    }

    public override void Dispose()
    {
        base.Dispose();
        UnsubscribeEvents();
    }
    private void HandleClickCloseForge()
    {
        View.CloseForge();
    }

}