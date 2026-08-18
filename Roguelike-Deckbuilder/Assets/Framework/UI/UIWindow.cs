using Cysharp.Threading.Tasks;
using UnityEngine;

namespace LitFramework.UI.Core.Window
{
    public abstract class UIWindow : UIBase
    {

        // ===== 由 UIService 调用 =====
        internal async UniTask OpenInternalAsync()
        {
            await OnOpenAsync();
        }

        internal void CloseInternal()
        {
            if (!IsActive) return;
            OnClose();
        }

        // ===== 子类重写 =====
        protected virtual UniTask OnOpenAsync() => UniTask.CompletedTask;
        protected virtual void OnClose() { }
    }

}
