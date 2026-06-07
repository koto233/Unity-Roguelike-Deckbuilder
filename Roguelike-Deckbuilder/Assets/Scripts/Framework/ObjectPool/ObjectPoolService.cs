using System.Collections.Generic;
using UnityEngine;

namespace Framework.ObjectPool
{
    /// <summary>
    /// 管理多个不同对象的池子
    /// </summary> 
    public class ObjectPoolService
    {
        private Dictionary<string, object> _pools = new Dictionary<string, object>();

        public void RegisterPool<T>(string poolName, ObjectPool<T> pool) where T : class
        {
            if (_pools.ContainsKey(poolName))
            {
                return;
            }
            _pools.Add(poolName, pool);
        }

        public T GetT<T>(string poolName) where T : class
        {
            if (_pools.TryGetValue(poolName, out var pool) && pool is ObjectPool<T> typedPool)
            {
                return typedPool.Get();
            }
            else
            {
                Debug.LogError($"对象池 {poolName} 不存在或类型不匹配");
                return null;
            }
        }
        public void ReturnT<T>(string poolName, T obj) where T : class
        {
            if (_pools.TryGetValue(poolName, out var pool) && pool is ObjectPool<T> typedPool)
            {
                typedPool.Return(obj);
            }
            else
            {
                Debug.LogError($"对象池 {poolName} 不存在或类型不匹配");
                if (obj is GameObject go)
                {
                    Object.Destroy(go);
                }
                else if (obj is Component comp)
                {
                    Object.Destroy(comp.gameObject);
                }
            }
        }

        public void Clear()
        {
            foreach (var pool in _pools)
            {
                if (pool.Value is ObjectPool<GameObject> goPool)
                {
                    goPool.Clear();
                }
            }
            _pools.Clear();
        }
    }
}