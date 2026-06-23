using System;
using UnityEngine;

namespace LitFramework.ObjectPool
{
    /// <summary>
    /// GameObject对象池
    /// </summary> 
    public class GameObjectPool
    {
        private readonly ObjectPool<GameObject> _pool;
        private readonly Transform _poolParent;
        private readonly GameObject _prefab;
        /// <summary>
        /// 创建对象池
        /// </summary>
        /// <param name="prefab">预制体</param>
        /// <param name="poolParent">池父对象</param>
        /// <param name="initialPoolSize">初始池大小</param>
        /// <param name="maxPoolSize">池最大容量</param>
        /// <param name="onGet">获取实例时的回调</param>
        /// <param name="onReturn">归还实例时的回调</param>
        public GameObjectPool(GameObject prefab, Transform poolParent = null, int initialPoolSize = 0, int maxPoolSize = 100, Action<GameObject> onGet = null, Action<GameObject> onReturn = null)
        {
            _prefab = prefab;
            _poolParent = poolParent;
            if (poolParent == null)
            {
                var parent = new GameObject($"{prefab.name}_Pool");
                parent.SetActive(false);
                _poolParent = parent.transform;
            }
            _pool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    GameObject go = GameObject.Instantiate(prefab);  // 保存实例引用
                    go.SetActive(false);                             // 设为非激活
                    go.transform.SetParent(_poolParent);
                    return go;                                       // 返回实例
                },
                onGet: go =>
                {
                    go.transform.SetParent(null);
                    go.SetActive(true);
                    onGet?.Invoke(go);
                },
                onReturn: go =>
                {
                    go.SetActive(false);
                    go.transform.SetParent(_poolParent);
                    onReturn?.Invoke(go);
                },
                onDestroy: go => GameObject.Destroy(go),
                initialPoolSize: initialPoolSize,
                maxPoolSize: maxPoolSize
            );
        }

        public GameObject Get()
        {
            return _pool.Get();
        }

        public void Return(GameObject instance)
        {
            _pool.Return(instance);
        }

        public void Clear()
        {
            _pool.Clear();
        }
    }
}