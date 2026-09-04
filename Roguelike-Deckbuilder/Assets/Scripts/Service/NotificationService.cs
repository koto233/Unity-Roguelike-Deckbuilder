using System;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.UI.Core.Service;

public class NotificationService
{
    private UIService _uiService;

    public NotificationService()
    {
        _uiService = ServiceLocator.Get<UIService>();
    }

    /// <summary>
    /// 显示浮动提示
    /// </summary>
    public async UniTask ShowToast(string message, float duration = 2f)
    {
        await _uiService.OpenAsync<ToastView, ToastData>(new ToastData { Message = message, Duration = duration });
    }

    /// <summary>
    /// 显示确认弹窗
    /// </summary>
    public async UniTask ShowConfirm(
        string title,
        string content,
        string confirmText = "确定",
        string cancelText = "取消",
        Action onConfirm = null,
        Action onCancel = null)
    {
        await _uiService.OpenAsync<ConfirmDialogView, ConfirmDialogData>(new ConfirmDialogData
        {
            Title = title,
            Content = content,
            ConfirmText = confirmText,
            CancelText = cancelText,
            OnConfirm = onConfirm,
            OnCancel = onCancel
        });
    }
}