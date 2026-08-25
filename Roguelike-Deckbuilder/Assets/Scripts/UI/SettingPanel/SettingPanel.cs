using System.Collections;
using System.Collections.Generic;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class SettingPanel : UIWindow
{
    public event System.Action OnClickContinue;
    public event System.Action OnClickGiveUp;
    public event System.Action OnClickQuit;
    public void Init()
    {

    }
    private void SubscribeEvents()
    {
        b_Continue.onClick.AddListener(() => OnClickContinue?.Invoke());
        b_GiveUp.onClick.AddListener(() => OnClickGiveUp?.Invoke());
        b_SaveAndQuit.onClick.AddListener(() => OnClickQuit?.Invoke());
    }
    void OnEnable()
    {
        SubscribeEvents();
    }
    void OnDisable()
    {
        b_Continue.onClick.RemoveAllListeners();
        b_GiveUp.onClick.RemoveAllListeners();
        b_SaveAndQuit.onClick.RemoveAllListeners();
    }
}
