using LitFramework.UI.Core.Window;

namespace LitFramework.UI.Core.Service
{
    public interface IPresenter
    {
        void Unbind();
    }
    // ============ 泛型接口继承基接口 ============
    public interface IPresenter<TView> : IPresenter where TView : UIWindow
    {
        void Bind(TView view);
    }
}