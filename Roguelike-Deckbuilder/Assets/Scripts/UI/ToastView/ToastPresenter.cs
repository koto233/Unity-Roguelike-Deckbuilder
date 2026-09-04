using System;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.UI.Core.Service;

public class ToastPresenter : BasePresenter<ToastView>, IHasData<ToastData>
{
    private UIService _uiService;
    private ToastData _data;
    public ToastPresenter(ToastView view) : base(view) { }

    public override void Init()
    {
        _uiService = ServiceLocator.Get<UIService>();
        // 设置初始位置为屏幕中心（假定锚点在中心）
        View.ResetState();
        Show().Forget();
    }

    /// <summary>
    /// 显示 Toast，播放动画，完成后自动关闭
    /// </summary>
    public async UniTask Show()
    {
        View.SetMessage(_data.Message);
        await View.PlayShowAnimation();
        _uiService.Close(View);
    }

    public override void Dispose()
    {
        base.Dispose();
    }

    public void SetData(ToastData data)
    {
        _data = data;
    }
}/// <summary>
/// Toast 浮动提示数据
/// </summary>
public class ToastData
{
    public string Message;
    public float Duration = 2f;
}