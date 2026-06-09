using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Framework.State
{
    public class StateMachine
    {
        private Dictionary<Type, IState> _states = new();
        private IState _currentState;
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

            var prevType = _currentState?.GetType();
            _currentState?.OnExit();
            _currentState = newState;
            _currentState.OnEnter();

            OnStateChanged?.Invoke(prevType, newStateType);
        }

        public void Destroy()
        {
            foreach (var state in _states.Values) state.OnDestroy();
            _states.Clear();
            _currentState = null;
        }
    }
}