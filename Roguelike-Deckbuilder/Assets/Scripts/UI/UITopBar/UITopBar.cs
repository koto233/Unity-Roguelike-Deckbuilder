using System.Collections;
using System.Collections.Generic;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class UITopBar : UIWindow
{
    public event System.Action OnClickSetting;
    public event System.Action OnClickMap;
    public event System.Action OnClickPile;
    public void Init()
    {
        SubscribeEvents();
    }
    private void SubscribeEvents()
    {
        b_SettingBtn.onClick.AddListener(() => OnClickSetting?.Invoke());
        b_MapBtn.onClick.AddListener(() => OnClickMap?.Invoke());
        b_PileBtn.onClick.AddListener(() => OnClickPile?.Invoke());
    }
    void OnDisable()
    {
        b_SettingBtn.onClick.RemoveAllListeners();
        b_MapBtn.onClick.RemoveAllListeners();
        b_PileBtn.onClick.RemoveAllListeners();
    }
}
