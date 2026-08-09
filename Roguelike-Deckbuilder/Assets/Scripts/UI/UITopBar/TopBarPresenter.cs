using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopBarPresenter
{
    private UITopBar _view;
    public TopBarPresenter(UITopBar view)
    {
        _view = view;
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        _view.OnClickSetting += HandleClickSetting;
        _view.OnClickMap += HandleClickMap;
        _view.OnClickPile += HandleClickPile;
    }

    private void HandleClickSetting()
    {


    }

    private void HandleClickMap()
    {

    }
    private void HandleClickPile()
    {

    }
}
