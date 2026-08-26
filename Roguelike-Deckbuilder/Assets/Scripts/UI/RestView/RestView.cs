using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class RestView : UIWindow
{
    public event Action OnClickContinue;
    public event Action OnClickForge;
    public event Action OnClickRest;
    public event Action OnClickCloseForge;
    private void OnEnable()
    {
        SubscribeEvents();
        b_ContinueButton.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void SubscribeEvents()
    {
        b_ContinueButton.onClick.AddListener(() => OnClickContinue?.Invoke());
        b_ForgeButton.onClick.AddListener(() => OnClickForge?.Invoke());
        b_RestButton.onClick.AddListener(() => OnClickRest?.Invoke());
        b_BackButton.onClick.AddListener(() => OnClickCloseForge?.Invoke());
    }

    private void UnsubscribeEvents()
    {
        b_ContinueButton.onClick.RemoveAllListeners();
        b_ForgeButton.onClick.RemoveAllListeners();
        b_RestButton.onClick.RemoveAllListeners();
        b_BackButton.onClick.RemoveAllListeners();
    }
    public void ShowContinue()
    {
        b_RestButton.gameObject.SetActive(false);
        b_ForgeButton.gameObject.SetActive(false);
        b_ContinueButton.gameObject.SetActive(true);
    }
    public void OpenForge()
    {
        b_ForgePanel.gameObject.SetActive(true);
    }
    public void CloseForge()
    {
        b_ForgePanel.gameObject.SetActive(false);
    }
}
