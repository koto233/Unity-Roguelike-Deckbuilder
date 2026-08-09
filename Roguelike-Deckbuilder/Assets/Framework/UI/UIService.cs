using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using LitFramework.Asset;
using LitFramework.UI.Core.Window;
using UnityEngine;
namespace LitFramework.UI.Core.Service
{
    public sealed class UIService : IDisposable
    {
        private readonly Dictionary<Type, UIConfig> _configs = new();
        private readonly Dictionary<Type, UIWindow> _opened = new();
        private Dictionary<Type, IPresenter> _presenters = new();
        private readonly Dictionary<UILayer, RectTransform> _layers = new();
        private IAssetService _assetManager;

        private IAssetService AssetManager =>
        _assetManager ??= ServiceLocator.Get<IAssetService>();
        public UIService()
        {
            foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
            {
                var go = new GameObject(layer.ToString());
                var rect = go.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 1);
                rect.offsetMin = Vector2.zero; // 对应 Left, Bottom
                rect.offsetMax = Vector2.zero; // 对应 Right, Top
                rect.pivot = new Vector2(0.5f, 0.5f);
                var uiRoot = GameRoot.Instance.UIRoot;
                rect.SetParent(uiRoot.transform, false);
                _layers[layer] = rect;
            }
        }

        public void Register<T>(string prefabPath, UILayer layer)
            where T : UIWindow
        {
            _configs[typeof(T)] = new UIConfig(prefabPath, layer);
        }

        // /// <summary>
        // /// UniTask版本异步打开窗口
        // /// </summary>
        // /// <typeparam name="T"></typeparam>
        // /// <param name="args"></param>
        // /// <returns></returns> 
        // public async UniTask<T> OpenAsync<T>(object args = null) where T : UIWindow
        // {
        //     var type = typeof(T);

        //     if (_opened.TryGetValue(type, out var existing))
        //     {
        //         return existing as T;
        //     }

        //     var cfg = _configs[type];
        //     var prefab = await AssetManager.LoadAsync<GameObject>(cfg.PrefabPath);
        //     var go = UnityEngine.Object.Instantiate(prefab, _layers[cfg.Layer]);
        //     var window = go.GetComponent<T>();
        //     // window.OnOpen(args);
        //     await window.OpenInternalAsync(args);
        //     _opened[type] = window;
        //     return window;
        // }
        /// <summary>
        /// 打开 UI，并自动创建 Presenter 绑定生命周期
        /// </summary>
        public async UniTask<TView> OpenAsync<TView, TPresenter>(object args = null)
            where TView : UIWindow
            where TPresenter : IPresenter<TView>, new() // 👈 约束 Presenter 可 new
        {
            var type = typeof(TView);

            // ✅ 如果已经打开，直接返回（不重复创建 Presenter）
            if (_opened.TryGetValue(type, out var existing))
            {
                return existing as TView;
            }

            // 1. 加载并实例化 View
            var cfg = _configs[type];
            var prefab = await AssetManager.LoadAsync<GameObject>(cfg.PrefabPath);
            var go = UnityEngine.Object.Instantiate(prefab, _layers[cfg.Layer]);
            var view = go.GetComponent<TView>();

            // 2. 创建 Presenter 并绑定 View
            var presenter = new TPresenter();
            presenter.Bind(view);
            _presenters[type] = presenter; // 存储 Presenter，方便后续管理
            // 3. 打开 View（传递参数）
            await view.OpenInternalAsync(args);

            // 4. 存储到已打开字典
            _opened[type] = view;

            return view;
        }
        public void Close<T>() where T : UIWindow
        {
            var type = typeof(T);
            if (_presenters.TryGetValue(type, out var presenter))
            {
                presenter.Unbind();
                _presenters.Remove(type);
            }
            if (_opened.TryGetValue(type, out var window))
            {
                window.CloseInternal();

                GameObject.Destroy(window.gameObject);
                _opened.Remove(type);
            }
        }

        public void Dispose()
        {
            foreach (var w in _opened.Values)
                GameObject.Destroy(w.gameObject);

            _opened.Clear();
        }
    }
}