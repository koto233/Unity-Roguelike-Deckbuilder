using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class UIBattleEnd : UIWindow
{
    public event Action OnSkipClick;


    void OnEnable()
    {
        SubscribeEvents();
    }
    void OnDisable()
    {
        UnsubscribeEvents();
    }
    private void SubscribeEvents()
    {
        b_SkipButton.onClick.AddListener(() => OnSkipClick?.Invoke());  
    }
    private void UnsubscribeEvents()
    {
        b_SkipButton.onClick.RemoveAllListeners();
     
    }
}
