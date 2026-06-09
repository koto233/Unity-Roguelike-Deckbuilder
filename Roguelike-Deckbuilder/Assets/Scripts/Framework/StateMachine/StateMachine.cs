using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Framework.State
{
    public class StateMachine
    {
        private Dictionary<Type, IState> _states = new Dictionary<Type, IState>();
        private IState _currentState;
        private Coroutine _coroutine;

        public event Action<Type, Type> OnStateChanged;

        public void RegisterState<T>(T state) where T : IState
        {
            if (!_states.ContainsKey(typeof(T)))
            {
                state.OnInit();
                _states.Add(typeof(T), state);
            }
            else
            {
                Debug.LogError($"{typeof(T)} 状态已存在，请勿重复添加。");
            }
        }

        public void Update()
        {
            _currentState?.OnUpdate();
        }

        private IEnumerator EnterCoroutine(ICoroutineState newState, Type prevType, Type newStateType)
        {
            yield return newState.OnEnterCoroutine();
            OnStateChanged?.Invoke(prevType, newStateType);
            _coroutine = null;
        }
        /// <summary>
        /// 切换状态（同步，适用于非协程状态）
        /// </summary>
        public void ChangeState<T>() where T : IState
        {
            var newStateType = typeof(T);
            if (!_states.TryGetValue(newStateType, out var newState))
            {
                Debug.LogError($"未注册状态: {newStateType.Name}");
                return;
            }

            // 停止当前协程（如果有）
            if (_coroutine != null)
                CoroutineRunner.Instance.StopCoroutine(_coroutine);

            var prevType = _currentState?.GetType();
            _currentState?.OnExit();
            _currentState = newState;
            _currentState.OnEnter();

            OnStateChanged?.Invoke(prevType, newStateType);
        }

        /// <summary>
        /// 切换状态（异步，支持协程）
        /// </summary>
        public void ChangeStateCoroutine<T>() where T : ICoroutineState
        {
            var newStateType = typeof(T);
            if (!_states.TryGetValue(newStateType, out var state))
            {
                Debug.LogError($"未注册协程状态: {newStateType.Name}");
                return;
            }
            var newState = state as ICoroutineState;

            // 停止当前协程
            if (_coroutine != null)
                CoroutineRunner.Instance.StopCoroutine(_coroutine);

            var prevType = _currentState?.GetType();
            _currentState?.OnExit();
            _currentState = newState;
            _currentState.OnEnter();
            // 启动协程进入
            _coroutine = CoroutineRunner.Instance.RunCoroutine(EnterCoroutine(newState, prevType, newStateType));
        }

    }
}