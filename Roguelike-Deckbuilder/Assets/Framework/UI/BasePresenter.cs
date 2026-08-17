using System;
using LitFramework.UI.Core.Window;

public abstract class BasePresenter : IDisposable
{
    public abstract void Dispose();
}
public abstract class BasePresenter<TView> : BasePresenter where TView : UIWindow
{
    protected TView View { get; private set; }

    protected BasePresenter(TView view)
    {
        View = view;
    }

    // 可选扩展点
    protected virtual void OnViewBound() { }

    public override void Dispose()
    {
        // 由派生类重写
    }
}