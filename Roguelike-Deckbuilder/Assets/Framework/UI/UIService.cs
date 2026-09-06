using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using LitFramework.Asset;
using LitFramework.UI.Core.Window;
using UnityEngine;

namespace LitFramework.UI.Core.Service
{
    public sealed class UIService : IDisposable
    {
        // ========== 配置 & 工厂 ==========
        private readonly Dictionary<Type, UIConfig> _configs = new();
        private readonly Dictionary<Type, Func<UIWindow, BasePresenter>> _factories = new();

        // ========== 运行时状态（每个类型只存在一个实例） ==========
        private readonly Dictionary<Type, WindowState> _opened = new();
        // 返回栈（存类型，按打开顺序入栈，关闭时移除）
        private readonly Stack<Type> _backStack = new();
        // 加载防重：Type -> 等待信号
        private readonly Dictionary<Type, UniTaskCompletionSource> _loading = new();

        private readonly Dictionary<UILayer, RectTransform> _layers = new();
        private IAssetService _assetService;
        private IAssetService AssetService => _assetService ??= ServiceLocator.Get<IAssetService>();

        private readonly struct WindowState
        {
            public readonly UIWindow View;
            public readonly BasePresenter Presenter;
            public readonly Action ReleaseAsset;
            public readonly bool PushToStack;

            public WindowState(UIWindow view, BasePresenter presenter, Action releaseAsset, bool pushToStack)
            {
                View = view;
                Presenter = presenter;
                ReleaseAsset = releaseAsset;
                PushToStack = pushToStack;
            }
        }

        public UIService()
        {
            foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
            {
                var go = new GameObject(layer.ToString());
                var rect = go.AddComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.SetParent(GameRoot.Instance.UIRoot.transform, false);
                _layers[layer] = rect;
            }
        }

        // ========== 注册 ==========

        /// <summary>
        /// 注册 UI 配置。单例模式下不再需要 allowMultiple 参数。
        /// </summary>
        /// <param name="pushToStack">是否加入返回栈（如页面加入，弹窗不加入）</param>
        public void Register<T>(string prefabPath, UILayer layer, bool pushToStack = true) where T : UIWindow
        {
            _configs[typeof(T)] = new UIConfig(prefabPath, layer, false, pushToStack);
        }

        /// <summary>
        /// 绑定 Presenter 工厂
        /// </summary>
        public void Bind<TView>(Func<TView, BasePresenter> factory) where TView : UIWindow
        {
            _factories[typeof(TView)] = window => factory((TView)window);
        }

        // ========== 打开窗口 ==========

        public async UniTask OpenAsync<TView>() where TView : UIWindow
        {
            await OpenCore<TView>(null, null);
        }

        public async UniTask OpenAsync<TView, TArg>(TArg arg) where TView : UIWindow
        {
            await OpenCore<TView>(arg, (p, a) =>
            {
                if (p is IHasData<TArg> d) d.SetData((TArg)a);
            });
        }

        private async UniTask OpenCore<TView>(object arg, Action<BasePresenter, object> inject) where TView : UIWindow
        {
            var type = typeof(TView);
            var cfg = GetConfig(type);

            // 1. 已打开：置顶并更新数据（若有参数）
            if (_opened.TryGetValue(type, out var exist))
            {
                Debug.Log($"[UIService] 窗口已存在，直接返回。Presenter: {exist.Presenter?.GetHashCode()}, View: {exist.View}");
                exist.View.transform.SetAsLastSibling();
                inject?.Invoke(exist.Presenter, arg);
                return;
            }

            // 2. 正在加载：等待同一个加载任务完成
            if (_loading.TryGetValue(type, out var loadingTask))
            {
                await loadingTask.Task;
                // 加载完成后再次尝试获取，如果已打开则更新数据
                if (_opened.TryGetValue(type, out var openedAfterLoad))
                {
                    openedAfterLoad.View.transform.SetAsLastSibling();
                    inject?.Invoke(openedAfterLoad.Presenter, arg);
                }
                return;
            }

            var completion = new UniTaskCompletionSource();
            _loading[type] = completion;

            GameObject go = null;
            try
            {
                // 3. 加载 & 实例化
                var prefab = await AssetService.LoadAsync<GameObject>(cfg.PrefabPath);
                go = UnityEngine.Object.Instantiate(prefab, _layers[cfg.Layer]);
                var view = go.GetComponent<TView>();
                if (view == null)
                    throw new InvalidOperationException($"Prefab {cfg.PrefabPath} 上缺少 {type.Name} 组件");

                // 4. 创建 Presenter
                if (!_factories.TryGetValue(type, out var factory))
                    throw new InvalidOperationException($"{type.Name} 未绑定 Presenter 工厂");

                var presenter = factory(view);

                // 5. 注入数据（在 Init 前完成，确保 Presenter 初始化时能使用数据）
                inject?.Invoke(presenter, arg);

                // 7. View 打开（播放动画等）
                await view.OpenInternalAsync();
                presenter.Init();
                // 8. 注册状态
                Action releaseAction = () => AssetService.Release(cfg.PrefabPath);
                var state = new WindowState(view, presenter, releaseAction, cfg.PushToStack);
                _opened[type] = state;

                if (cfg.PushToStack)
                    _backStack.Push(type);

                completion.TrySetResult();
            }
            catch (Exception ex)
            {
                // 清理半成品
                if (go != null)
                    UnityEngine.Object.Destroy(go);
                completion.TrySetException(ex);
                throw;
            }
            finally
            {
                _loading.Remove(type);
            }
        }

        // ========== 关闭窗口 ==========

        public void Close<TView>() where TView : UIWindow
        {
            CloseCore(typeof(TView));
        }

        public void Close(UIWindow view)
        {
            if (view == null) return;
            var type = view.GetType();
            if (_opened.TryGetValue(type, out var state) && state.View == view)
                CloseCore(type);
        }

        private void CloseCore(Type type)
        {
            if (!_opened.TryGetValue(type, out var state)) return;

            try
            {
                // 1. 从返回栈移除
                if (state.PushToStack)
                {
                    var temp = new Stack<Type>();
                    while (_backStack.Count > 0)
                    {
                        var t = _backStack.Pop();
                        if (t != type) temp.Push(t);
                    }
                    while (temp.Count > 0) _backStack.Push(temp.Pop());
                }

                // 2. 关闭 View（可能触发事件，此时 Presenter 仍可用）
                state.View.CloseInternal();

                // 3. 释放 Presenter（内部应处理 View 可能为 null 的情况）
                if (state.Presenter is IDisposable disposable)
                    disposable.Dispose();

                // 4. 销毁 GameObject
                if (state.View != null && state.View.gameObject != null)
                    UnityEngine.Object.Destroy(state.View.gameObject);

                // 5. 释放资源引用
                state.ReleaseAsset?.Invoke();
            }
            finally
            {
                // 无论如何都要移除状态，防止残留
                _opened.Remove(type);
            }
        }
        // ========== 返回栈 ==========

        public bool CanGoBack => _backStack.Count > 0;

        public void GoBack()
        {
            if (_backStack.Count == 0) return;
            var type = _backStack.Pop();
            CloseCore(type);
        }

        // ========== 查询 ==========

        public TPresenter GetPresenter<TPresenter>() where TPresenter : BasePresenter
        {
            return _opened.Values
                .Select(s => s.Presenter)
                .OfType<TPresenter>()
                .FirstOrDefault();
        }

        public bool IsOpen<TView>() where TView : UIWindow
        {
            return _opened.ContainsKey(typeof(TView));
        }

        public void Dispose()
        {
            // 逆序关闭，避免依赖问题
            var types = _opened.Keys.ToList();
            for (int i = types.Count - 1; i >= 0; i--)
                CloseCore(types[i]);

            _opened.Clear();
            _backStack.Clear();
            _loading.Clear();
        }

        // ========== 工具 ==========

        private UIConfig GetConfig(Type type)
        {
            if (!_configs.TryGetValue(type, out var cfg))
                throw new InvalidOperationException($"{type.Name} 未注册 UIConfig，先调用 Register");
            return cfg;
        }
    }
}