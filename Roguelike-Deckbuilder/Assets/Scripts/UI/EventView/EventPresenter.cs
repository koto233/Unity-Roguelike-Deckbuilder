using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.UI.Core.Service;
using UnityEngine;

public class EventPresenter : BasePresenter<EventView>
{
    private UIService _uiService;


    public EventPresenter(EventView view) : base(view)
    {
    }



    public override void Init()
    {
        _uiService = ServiceLocator.Get<UIService>();
        SubscribeEvents();
    }
    private void SubscribeEvents()
    {
        
    }



    private void UnsubscribeEvents()
    {

    }
    public override void Dispose()
    {
        base.Dispose();
        UnsubscribeEvents();
    }


}
