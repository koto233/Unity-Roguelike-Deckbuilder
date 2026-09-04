using System;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.UI.Core.Service;

public class ConfirmDialogPresenter : BasePresenter<ConfirmDialogView>, IHasData<ConfirmDialogData>
{
    private ConfirmDialogData _data;
    private UIService _uiService;
    private Action _onConfirm;
    private Action _onCancel;

    public ConfirmDialogPresenter(ConfirmDialogView view) : base(view) { }

    public override void Init()
    {
        _uiService = ServiceLocator.Get<UIService>();
        SubscribeEvents();
        View.SetData(_data.Title, _data.Content, _data.ConfirmText, _data.CancelText);
    }

    private void SubscribeEvents()
    {
        View.OnConfirm += HandleConfirm;
        View.OnCancel += HandleCancel;
    }

    private void UnsubscribeEvents()
    {
        View.OnConfirm -= HandleConfirm;
        View.OnCancel -= HandleCancel;
    }

    private void HandleConfirm()
    {
        _onConfirm?.Invoke();
        _uiService.Close(View);
    }

    private void HandleCancel()
    {
        _onCancel?.Invoke();
        _uiService.Close(View);
    }

    public override void Dispose()
    {
        UnsubscribeEvents();
        base.Dispose();
    }

    public void SetData(ConfirmDialogData data)
    {
        _data = data;
    }
}/// <summary>
/// 确认弹窗数据
/// </summary>
public class ConfirmDialogData
{
    public string Title;
    public string Content;
    public string ConfirmText = "确定";
    public string CancelText = "取消";

    [NonSerialized] // 防止被存档系统误序列化
    public Action OnConfirm;

    [NonSerialized]
    public Action OnCancel;
}