// Scripts/Framework/CoroutineRunner.cs
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LitFramework
{
    // 1. 定义各种生命周期接口（按需实现，避免污染）
    public interface ITickable { void Tick(float deltaTime); }
    public interface IFixedTickable { void FixedTick(float fixedDeltaTime); }
    public interface ILateTickable { void LateTick(float deltaTime); }
    public interface IOnDestroyNotify { void OnDestroyNotify(); }

    /// <summary>
    /// Mono服务，用于非 MonoBehaviour提供mono功能
    /// </summary>
    public class MonoService : MonoBehaviour
    {
        // 使用 HashSet 或 List，这里用 List + 加锁/缓存来处理动态增删
        private List<ITickable> _tickables = new List<ITickable>();
        private List<IFixedTickable> _fixedTickables = new List<IFixedTickable>();
        private List<ILateTickable> _lateTickables = new List<ILateTickable>();
        private List<IOnDestroyNotify> _destroyNotifies = new List<IOnDestroyNotify>();

        private readonly Queue<Action> _mainThreadActions = new Queue<Action>(); // 可选：主线程调度

        private static MonoService _instance;
        public static MonoService Instance => _instance;

        private void Awake()
        {
            if (_instance != null) Destroy(gameObject);
            _instance = this;
            DontDestroyOnLoad(gameObject); // 常驻

            // 注册到 ServiceLocator，方便任何纯 C# 类获取
            ServiceLocator.Register<MonoService>(this);
        }

        // -------- 注册方法（线程安全考量：由于都在主线程，直接操作） --------
        public void AddUpdate(ITickable obj) { if (!_tickables.Contains(obj)) _tickables.Add(obj); }
        public void RemoveUpdate(ITickable obj) { if (_tickables.Contains(obj)) _tickables.Remove(obj); }

        public void AddFixedUpdate(IFixedTickable obj) { if (!_fixedTickables.Contains(obj)) _fixedTickables.Add(obj); }
        public void RemoveFixedUpdate(IFixedTickable obj) { if (_fixedTickables.Contains(obj)) _fixedTickables.Remove(obj); }

        public void AddLateUpdate(ILateTickable obj) { if (!_lateTickables.Contains(obj)) _lateTickables.Add(obj); }
        public void RemoveLateUpdate(ILateTickable obj) { if (_lateTickables.Contains(obj)) _lateTickables.Remove(obj); }

        public void AddDestroyNotify(IOnDestroyNotify obj) { if (!_destroyNotifies.Contains(obj)) _destroyNotifies.Add(obj); }
        public void RemoveDestroyNotify(IOnDestroyNotify obj) { if (_destroyNotifies.Contains(obj)) _destroyNotifies.Remove(obj); }

        // 为纯 C# 类提供启动协程的能力（极其实用）
        public Coroutine RunCoroutine(System.Collections.IEnumerator routine)
        {
            return StartCoroutine(routine);
        }

        // -------- Unity 生命周期回调（驱动所有注册对象） --------
        private void Update()
        {
            float delta = Time.deltaTime;
            foreach (var obj in _tickables) obj.Tick(delta);
        }

        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;
            foreach (var obj in _fixedTickables) obj.FixedTick(delta);
        }

        private void LateUpdate()
        {
            float delta = Time.deltaTime;
            foreach (var obj in _lateTickables) obj.LateTick(delta);
        }

        private void OnDestroy()
        {
            // 通知所有注册对象销毁（用于清理事件订阅等）
            foreach (var obj in _destroyNotifies) obj.OnDestroyNotify();
            _instance = null;
        }
    }
}