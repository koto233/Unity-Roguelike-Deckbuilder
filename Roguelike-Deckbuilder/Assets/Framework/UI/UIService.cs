using System;
using System.Collections.Generic;
using LitFramework.Asset;
using LitFramework.UI.Core.Window;
using UnityEngine;
namespace LitFramework.UI.Core.Service
{
    public sealed class UIService : IDisposable
    {
        private readonly Dictionary<Type, UIConfig> _configs = new();
        private readonly Dictionary<Type, UIWindow> _opened = new();
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

        public T OpenUI<T>(IUIArgs args = null)
            where T : UIWindow
        {
            var type = typeof(T);

            if (_opened.TryGetValue(type, out var existing))
                return existing as T;

            var cfg = _configs[type];
            Debug.Log($"加载窗口: {cfg.PrefabPath}");
            var prefab = AssetManager.Load<GameObject>(cfg.PrefabPath);
            var go = UnityEngine.Object.Instantiate(prefab, _layers[cfg.Layer]);

            var window = go.GetComponent<T>();
            window.OnOpen(args);

            _opened[type] = window;
            return window;
        }
        public void OpenAsync<T>(IUIArgs args = null, Action<T> onCompleted = null)
                    where T : UIWindow
        {
            var type = typeof(T);

            if (_opened.TryGetValue(type, out var existing))
            {
                onCompleted?.Invoke(existing as T);
                return;
            }

            var cfg = _configs[type];
            AssetManager.LoadAsync<GameObject>(cfg.PrefabPath, prefab =>
            {
                var go = UnityEngine.Object.Instantiate(prefab, _layers[cfg.Layer]);
                var window = go.GetComponent<T>();
                window.OnOpen(args);
                _opened[type] = window;
                onCompleted?.Invoke(window);
            });
        }
        public void Close<T>() where T : UIWindow
        {
            var type = typeof(T);

            if (_opened.TryGetValue(type, out var window))
            {
                window.OnClose();
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