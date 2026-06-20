using Cysharp.Threading.Tasks;

namespace LitFramework.UI.Core.Window
{
    public abstract class UIWindow : UIBase
    {
        // public virtual void OnInit() { }
        // public virtual UniTask OnOpenAsync() { return UniTask.CompletedTask; }
        // public virtual void OnOpen(object args) { }
        // public virtual void OnClose() { }
        public bool IsShown { get; private set; }

        // ===== 由 UIService 调用 =====
        internal async UniTask OpenInternalAsync(object param)
        {
            if (IsShown) return;
            IsShown = true;
            // Show();  // SetActive(true)
            await OnOpenAsync(param);
        }

        internal void CloseInternal()
        {
            if (!IsShown) return;
            IsShown = false;
            OnClose();
            // Hide();  // SetActive(false)
        }

        // ===== 子类重写 =====
        protected virtual UniTask OnOpenAsync(object param) => UniTask.CompletedTask;
        protected virtual void OnClose() { }
    }

}
