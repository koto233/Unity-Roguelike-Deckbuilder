using System.Collections.Generic;
using UnityEngine;

namespace LitFramework.ObjectPool
{
    public class ObjectPoolService
    {
        private readonly Dictionary<string, object> _pools = new Dictionary<string, object>();

        private static string GetKey<T>() => typeof(T).Name;

        public void RegisterPool<T>(ObjectPool<T> pool) where T : class
        {
            RegisterPool(GetKey<T>(), pool);
        }

        public void RegisterPool<T>(string poolName, ObjectPool<T> pool) where T : class
        {
            if (_pools.ContainsKey(poolName))
                return;
            _pools.Add(poolName, pool);
        }

        public void RegisterGameObjectPool<T>(GameObject prefab, int initialPoolSize = 0, int maxPoolSize = 100) where T : Component
        {
            RegisterGameObjectPool(GetKey<T>(), prefab, initialPoolSize, maxPoolSize);
        }

        private void RegisterGameObjectPool(string poolName, GameObject prefab, int initialPoolSize = 0, int maxPoolSize = 100)
        {
            if (_pools.ContainsKey(poolName))
                return;
            _pools.Add(poolName, new GameObjectPool(prefab, null, initialPoolSize, maxPoolSize));
        }

        public T Get<T>() where T : class
        {
            return GetT<T>(GetKey<T>());
        }

        public T Get<T>(string poolName) where T : class
        {
            return GetT<T>(poolName);
        }

        public GameObject GetGameObject<T>() where T : Component
        {
            return GetGameObject(GetKey<T>());
        }

        private GameObject GetGameObject(string poolName)
        {
            if (_pools.TryGetValue(poolName, out var pool) && pool is GameObjectPool typedPool)
            {
                return typedPool.Get();
            }
            Debug.LogError($"GameObject对象池 {poolName} 不存在");
            return null;
        }

        public T GetComponent<T>() where T : Component
        {
            var go = GetGameObject<T>();
            return go?.GetComponent<T>();
        }

        public void Return<T>(T obj) where T : class
        {
            ReturnT(GetKey<T>(), obj);
        }

        public void Return<T>(string poolName, T obj) where T : class
        {
            ReturnT(poolName, obj);
        }

        public void ReturnGameObject<T>(GameObject go) where T : Component
        {
            ReturnGameObject(GetKey<T>(), go);
        }

        private void ReturnGameObject(string poolName, GameObject go)
        {
            if (_pools.TryGetValue(poolName, out var pool) && pool is GameObjectPool typedPool)
            {
                typedPool.Return(go);
                return;
            }
            Debug.LogError($"GameObject对象池 {poolName} 不存在");
            Object.Destroy(go);
        }

        private T GetT<T>(string poolName) where T : class
        {
            if (_pools.TryGetValue(poolName, out var pool) && pool is ObjectPool<T> typedPool)
            {
                return typedPool.Get();
            }
            Debug.LogError($"对象池 {poolName} 不存在或类型不匹配");
            return null;
        }

        private void ReturnT<T>(string poolName, T obj) where T : class
        {
            if (_pools.TryGetValue(poolName, out var pool) && pool is ObjectPool<T> typedPool)
            {
                typedPool.Return(obj);
                return;
            }
            Debug.LogError($"对象池 {poolName} 不存在或类型不匹配");
            if (obj is GameObject go)
                Object.Destroy(go);
            else if (obj is Component comp)
                Object.Destroy(comp.gameObject);
        }

        public void Clear()
        {
            foreach (var pool in _pools.Values)
            {
                if (pool is ObjectPool<object> typedPool)
                    typedPool.Clear();
                else if (pool is GameObjectPool goPool)
                    goPool.Clear();
            }
            _pools.Clear();
        }
    }
}
