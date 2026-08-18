using System;
using LitFramework.UI.Core.Window;

public abstract class BasePresenter : IDisposable
{
    public abstract void Init();
    public virtual void Dispose() { }
}
public abstract class BasePresenter<TView> : BasePresenter where TView : UIBase
{
    protected TView View { get; }

    protected BasePresenter(TView view)
    {
        View = view;
    }

}