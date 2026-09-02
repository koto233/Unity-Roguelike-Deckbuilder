using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class TopBar : UIWindow
{
    public event System.Action OnClickSetting;
    public event System.Action OnClickMap;
    public event System.Action OnClickDeck;

    public void Init()
    {

    }
    private void SubscribeEvents()
    {
        b_SettingBtn.onClick.AddListener(() => OnClickSetting?.Invoke());
        b_MapBtn.onClick.AddListener(() => OnClickMap?.Invoke());
        b_DeckBtn.onClick.AddListener(() => OnClickDeck?.Invoke());
    }
    void OnEnable()
    {
        SubscribeEvents();
    }
    void OnDisable()
    {
        b_SettingBtn.onClick.RemoveAllListeners();
        b_MapBtn.onClick.RemoveAllListeners();
        b_DeckBtn.onClick.RemoveAllListeners();
    }
    public void RefreshCoin(int oldCoin, int newCoin)
    {
        NumberAnimator.Play(b_CoinText, oldCoin, newCoin, 0.5f);
    }
    public void RefreshHp(int currentHp, int maxHp)
    {
        b_HpText.SetText(currentHp + "/" + maxHp);
    }
}
