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

        // ========== 运行时状态 ==========
        // 用 InstanceID 做 Key，支持同类型多实例堆叠
        private readonly Dictionary<int, WindowState> _opened = new();
        // 单实例窗口快速查找：Type -> InstanceID
        private readonly Dictionary<Type, int> _singleInstance = new();
        // 返回栈（只存需要返回的窗口 InstanceID）
        private readonly Stack<int> _backStack = new();
        // 正在加载中（防重复点击）：Type -> 等待信号
        private readonly Dictionary<Type, UniTaskCompletionSource> _loading = new();

        private readonly Dictionary<UILayer, RectTransform> _layers = new();
        private IAssetService _assetService;
        private IAssetService AssetService => _assetService ??= ServiceLocator.Get<IAssetService>();

        // 轻量内部结构，不用类，省 GC
        private readonly struct WindowState
        {
            public readonly UIWindow View;
            public readonly BasePresenter Presenter;
            public readonly Action ReleaseAsset; // 资源释放回调
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

        // ========== 注册（启动时一次） ==========

        /// <summary>
        /// 注册 UI 配置
        /// </summary>
        /// <param name="allowMultiple">是否允许同类型堆叠（如：同时开两个背包）</param>
        /// <param name="pushToStack">是否加入返回栈（如：页面加入，弹窗不加入）</param>
        public void Register<T>(string prefabPath, UILayer layer, bool allowMultiple = false, bool pushToStack = true)
            where T : UIWindow
        {
            _configs[typeof(T)] = new UIConfig(prefabPath, layer, allowMultiple, pushToStack);
        }

        /// <summary>
        /// 绑定 Presenter 工厂（替代反射，编译期检查）
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

            // 1. 单实例检查
            if (!cfg.AllowMultiple && _singleInstance.TryGetValue(type, out var existId))
            {
                if (_opened.TryGetValue(existId, out var exist))
                {
                    exist.View.transform.SetAsLastSibling();
                    return;
                }
            }

            // 2. 防重复点击
            if (_loading.TryGetValue(type, out var loadingTask))
            {
                await loadingTask.Task;
                return;
            }

            var completion = new UniTaskCompletionSource();
            _loading[type] = completion;

            try
            {
                // 3. 加载 & 实例化
                var prefab = await AssetService.LoadAsync<GameObject>(cfg.PrefabPath);
                var go = UnityEngine.Object.Instantiate(prefab, _layers[cfg.Layer]);
                var view = go.GetComponent<TView>();
                if (view == null)
                    throw new InvalidOperationException($"Prefab {cfg.PrefabPath} 上缺少 {type.Name} 组件");

                // 4. 创建 Presenter
                if (!_factories.TryGetValue(type, out var factory))
                    throw new InvalidOperationException($"{type.Name} 未绑定 Presenter 工厂");

                var presenter = factory(view);

                // 5. 注入数据（有参时）
                inject?.Invoke(presenter, arg);

                // 6. 统一初始化
                presenter.Init();
                await view.OpenInternalAsync();

                // 7. 记录状态
                var instanceId = go.GetInstanceID();
                Action releaseAction = () => AssetService.Release(cfg.PrefabPath);

                var state = new WindowState(view, presenter, releaseAction, cfg.PushToStack);
                _opened[instanceId] = state;

                if (!cfg.AllowMultiple)
                    _singleInstance[type] = instanceId;
                if (cfg.PushToStack)
                    _backStack.Push(instanceId);

                completion.TrySetResult();
            }
            catch (Exception ex)
            {
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
            var type = typeof(TView);
            if (!_singleInstance.TryGetValue(type, out var id)) return;
            CloseCore(id);
        }

        /// <summary>
        /// 通过 View 实例关闭（用于多实例窗口）
        /// </summary>
        public void Close(UIWindow view)
        {
            if (view == null) return;
            CloseCore(view.gameObject.GetInstanceID());
        }

        private void CloseCore(int instanceId)
        {
            if (!_opened.TryGetValue(instanceId, out var state)) return;

            // 从返回栈移除
            if (state.PushToStack)
            {
                var temp = new Stack<int>();
                while (_backStack.Count > 0)
                {
                    var id = _backStack.Pop();
                    if (id != instanceId) temp.Push(id);
                }
                while (temp.Count > 0) _backStack.Push(temp.Pop());
            }

            // 清理单实例映射
            _singleInstance.Remove(state.View.GetType());

            // Presenter 释放
            if (state.Presenter is IDisposable disposable)
                disposable.Dispose();

            // View 关闭
            state.View.CloseInternal();

            // 销毁 GameObject
            UnityEngine.Object.Destroy(state.View.gameObject);

            // 释放资源引用（防止内存泄漏）
            state.ReleaseAsset?.Invoke();

            _opened.Remove(instanceId);
        }

        // ========== 返回栈（安卓返回键 / 页面返回） ==========

        public bool CanGoBack => _backStack.Count > 0;

        public void GoBack()
        {
            if (_backStack.Count == 0) return;
            var id = _backStack.Pop();
            CloseCore(id);
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
            return _singleInstance.ContainsKey(typeof(TView));
        }

        public void Dispose()
        {
            // 逆序关闭，避免父子依赖问题
            foreach (var id in _opened.Keys.ToList())
                CloseCore(id);

            _opened.Clear();
            _singleInstance.Clear();
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