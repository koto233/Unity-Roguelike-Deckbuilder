using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class EventView : UIWindow
{

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void SubscribeEvents()
    {
        // b_ContinueButton.onClick.AddListener(() => OnClickContinue?.Invoke());
    }

    private void UnsubscribeEvents()
    {
        // b_ContinueButton.onClick.RemoveAllListeners();
    }
}
