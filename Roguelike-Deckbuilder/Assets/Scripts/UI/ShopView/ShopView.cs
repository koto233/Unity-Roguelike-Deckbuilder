using System;
using LitFramework.UI.Core.Window;

public partial class ShopView : UIWindow
{
    public event Action OnClickContinue;

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
        b_ContinueButton.onClick.AddListener(() => OnClickContinue?.Invoke());
    }

    private void UnsubscribeEvents()
    {
        b_ContinueButton.onClick.RemoveAllListeners();
    }
}
