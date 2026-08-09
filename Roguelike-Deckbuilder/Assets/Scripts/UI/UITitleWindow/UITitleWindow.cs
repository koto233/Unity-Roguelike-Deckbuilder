using LitFramework;
using LitFramework.Asset;
using LitFramework.FSM.Procedure;
using LitFramework.UI.Core.Service;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class UITitleWindow : UIWindow
{
    public event System.Action OnClickStart;
    public event System.Action OnClickContinue;

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
        b_StartButton.onClick.AddListener(() => OnClickStart?.Invoke());
        b_ContinueButton.onClick.AddListener(() => OnClickContinue?.Invoke());
    }
    private void UnsubscribeEvents()
    {
        b_StartButton.onClick.RemoveAllListeners();
        b_ContinueButton.onClick.RemoveAllListeners();
    }


}
