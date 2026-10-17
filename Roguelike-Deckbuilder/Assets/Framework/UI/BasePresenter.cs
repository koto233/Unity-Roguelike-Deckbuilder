using System;
using Cysharp.Threading.Tasks;
using LitFramework.UI.Core.Window;

public abstract class BasePresenter : IDisposable
{
    /// <summary>
    /// 窗口打开后初始化。UIService.OpenAsync 会 await 本方法，
    /// 所以返回完成时应保证 View 及其依赖资源（子视图、图标等）全部就绪。
    /// 没有异步初始化需求的 Presenter 返回 UniTask.CompletedTask 即可。
    /// </summary>
    internal async UniTask InitInternalAsync()
    {
        await OnInitAsync();
    }
    protected virtual UniTask OnInitAsync() => UniTask.CompletedTask;
    public virtual void Dispose() { }
}
public abstract class BasePresenter<TView> : BasePresenter where TView : UIBase
{
    protected TView View { get; private set; }

    protected BasePresenter(TView view)
    {
        View = view;
    }
    public override void Dispose()
    {
        View = null;  // 断开引用
        base.Dispose();
    }
}
