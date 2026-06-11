using System;
using System.Collections.Generic;

namespace LitFramework.ObjectPool
{
    public class ObjectPool<T> where T : class
    {
        private readonly Stack<T> _pool = new();
        private readonly Func<T> _createFunc; // 创建实例的方法
        private readonly Action<T> _onGet; // 获取实例时的回调
        private readonly Action<T> _onReturn; // 归还实例时的回调
        private readonly Action<T> _onDestroy; // 销毁实例时的回调
        private int _maxPoolSize;
        public int MaxPoolSize => _maxPoolSize;
        private int CurrentPoolSize => _pool.Count;
        /// <summary>
        ///  创建一个对象池
        /// </summary>
        /// <param name="createFunc">创建实例的方法</param>
        /// <param name="onGet">获取实例时的回调</param>
        /// <param name="onReturn">归还实例时的回调</param>
        /// <param name="onDestroy">销毁实例时的回调</param>
        /// <param name="initialPoolSize">初始池大小</param>
        /// <param name="maxPoolSize">池最大容量</param>
        public ObjectPool(Func<T> createFunc, Action<T> onGet = null, Action<T> onReturn = null, Action<T> onDestroy = null, int initialPoolSize = 0, int maxPoolSize = 100)
        {
            _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            _onGet = onGet;
            _onReturn = onReturn;
            _onDestroy = onDestroy;
            _maxPoolSize = maxPoolSize;

            // 预先创建一些实例
            for (int i = 0; i < initialPoolSize; i++)
            {
                _pool.Push(_createFunc());
            }
        }
        public T Get()
        {
            T instance;
            if (_pool.Count > 0)
            {
                instance = _pool.Pop();
            }
            else
            {
                instance = _createFunc();
            }

            _onGet?.Invoke(instance);
            return instance;
        }
        public void Return(T instance)
        {
            if (instance == null)
            {
                return;
            }
            (instance as IPoolable)?.OnRecycle();
            if (CurrentPoolSize < _maxPoolSize)
            {
                _pool.Push(instance);
                _onReturn?.Invoke(instance);
            }
            else
            {
                _onDestroy?.Invoke(instance);
                (instance as IDisposable)?.Dispose();
            }
        }

        public void Clear()
        {
            while (_pool.Count > 0)
            {
                var instance = _pool.Pop();
                _onDestroy?.Invoke(instance);
                (instance as IDisposable)?.Dispose();
            }
        }
    }
}