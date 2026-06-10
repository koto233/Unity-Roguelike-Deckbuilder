using System;
using System.Collections.Generic;
using LitFramework.AssetManager;
using LitFramework.UI.Core.Window;
using UnityEngine;
namespace LitFramework.UI.Core.Service
{
    public sealed class UIService : IDisposable
    {
        private readonly IAssetManager _loader;
        private readonly Dictionary<Type, UIConfig> _configs = new();
        private readonly Dictionary<Type, UIWindow> _opened = new();
        private readonly Dictionary<UILayer, RectTransform> _layers = new();

        public UIService(Canvas rootCanvas, IAssetManager assetManager)
        {
            _loader = assetManager;
            foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
            {
                var go = new GameObject(layer.ToString());
                var rect = go.AddComponent<RectTransform>();
                rect.SetParent(rootCanvas.transform, false);
                _layers[layer] = rect;
            }
        }

        public void Register<T>(string prefabPath, UILayer layer)
            where T : UIWindow
        {
            _configs[typeof(T)] = new UIConfig(prefabPath, layer);
        }

        public T Open<T>(IUIArgs args = null)
            where T : UIWindow
        {
            var type = typeof(T);

            if (_opened.TryGetValue(type, out var existing))
                return existing as T;

            var cfg = _configs[type];
            var prefab = _loader.Load<GameObject>(cfg.PrefabPath);
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
            _loader.LoadAsync<GameObject>(cfg.PrefabPath, prefab =>
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