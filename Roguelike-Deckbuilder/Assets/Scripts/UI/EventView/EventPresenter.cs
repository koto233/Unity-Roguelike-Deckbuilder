using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.UI.Core.Service;
using UnityEngine;

public class EventPresenter : BasePresenter<EventView>
{
    private UIService _uiService;


    public EventPresenter(EventView view) : base(view)
    {
    }



    protected override UniTask OnInitAsync()
    {
        _uiService = ServiceLocator.Get<UIService>();
        SubscribeEvents();
        return UniTask.CompletedTask;
    }
    private void SubscribeEvents()
    {

    }



    private void UnsubscribeEvents()
    {

    }
    public override void Dispose()
    {

        UnsubscribeEvents();
        base.Dispose();
    }


}
