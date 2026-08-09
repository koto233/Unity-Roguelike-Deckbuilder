using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingPresenter
{
    private UISetting _view;
    public SettingPresenter(UISetting view)
    {
        _view = view;
        SubscribeEvents();
    }
    private void SubscribeEvents()
    {

    }
}
