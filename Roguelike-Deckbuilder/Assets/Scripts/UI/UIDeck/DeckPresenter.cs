using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LitFramework;
using LitFramework.UI.Core.Service;

public class DeckPresenter : BasePresenter<UIDeck>
{
    public DeckPresenter(UIDeck view) : base(view)
    {
    }

    public override void Init()
    {

        SubscribeEvents();
        var globalData = ServiceLocator.Get<PlayerDataService>();
        View.SpawnCardInList(globalData.DeckCardIds);
    }

    private void SubscribeEvents()
    {
        View.OnClickBack += ClickBack;
    }



    private void UnsubscribeEvents()
    {
        View.OnClickBack -= ClickBack;
    }
    private void ClickBack()
    {
        ServiceLocator.Get<UIService>().Close<UIDeck>();
    }
    public override void Dispose()
    {
        UnsubscribeEvents();
        base.Dispose();
    }
}
