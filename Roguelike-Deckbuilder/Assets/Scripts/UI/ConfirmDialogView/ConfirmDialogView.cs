using System.Collections;
using System.Collections.Generic;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class ConfirmDialogView : UIWindow
{
    public event System.Action OnConfirm;
    public event System.Action OnCancel;

    private void OnEnable()
    {
        b_ConfirmButton.onClick.AddListener(() => OnConfirm?.Invoke());
        b_CancelButton.onClick.AddListener(() => OnCancel?.Invoke());
    }

    private void OnDisable()
    {
        b_ConfirmButton.onClick.RemoveAllListeners();
        b_CancelButton.onClick.RemoveAllListeners();
    }

    public void SetData(string title, string content, string confirmText = "确定", string cancelText = "取消")
    {
        b_Title.SetText(title);
        b_Desc.SetText(content);
        b_ConfirmText.text = confirmText;
        b_CancelText.text = cancelText;
    }

}
