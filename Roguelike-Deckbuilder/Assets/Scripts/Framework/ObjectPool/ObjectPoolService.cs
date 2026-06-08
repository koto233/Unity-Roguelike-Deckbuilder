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
        /// <summary>
        /// 注册泛型对象池
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="poolName">池子名称</param>
        /// <param name="pool">对象池</param>
        public void RegisterPool<T>(string poolName, ObjectPool<T> pool) where T : class
        {
            if (_pools.ContainsKey(poolName))
            {
                return;
            }
            _pools.Add(poolName, pool);
        }
        /// <summary>
        /// 注册GameObject对象池
        /// </summary>
        /// <param name="poolName">池子名称</param>
        /// <param name="pool">对象池</param>
        public void RegisterGameObjectPool(string poolName, GameObjectPool pool)
        {
            if (_pools.ContainsKey(poolName))
            {
                return;
            }
            _pools.Add(poolName, pool);
        }
        /// <summary>
        /// 获取泛型对象池中的对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="poolName">池子名称</param>
        /// <returns>对象</returns>
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
        /// <summary>
        /// 获取GameObject对象池中的对象
        /// </summary>
        /// <param name="poolName">池子名称</param>
        /// <returns>对象</returns>
        /// </summary>
        public GameObject GetGameObject(string poolName)
        {
            if (_pools.TryGetValue(poolName, out var pool) && pool is GameObjectPool typedPool)
            {
                return typedPool.Get();
            }
            else
            {
                Debug.LogError($"GameObject对象池 {poolName} 不存在或类型不匹配");
                return null;
            }
        }
        /// <summary>
        /// 归还泛型对象池中的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
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
        /// <summary>
        /// 归还GameObject对象池中的对象
        /// </summary>
        /// <param name="poolName"></param>
        /// <param name="go"></param> 
        public void ReturnGameObject(string poolName, GameObject go)
        {
            if (_pools.TryGetValue(poolName, out var pool) && pool is GameObjectPool typedPool)
            {
                typedPool.Return(go);
            }
            else
            {
                Debug.LogError($"对象池 {poolName} 不存在或类型不匹配");
                Object.Destroy(go);
            }
        }
        public void Clear()
        {
            foreach (var pool in _pools)
            {
                if (pool.Value is ObjectPool<Object> typedPool)
                {
                    typedPool.Clear();
                }
                else if (pool.Value is GameObjectPool goPool)
                {
                    goPool.Clear();
                }
            }
            _pools.Clear();
        }
    }
}