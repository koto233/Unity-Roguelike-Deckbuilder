using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LitFramework.FSM
{
    public class StateMachine
    {
        private Dictionary<Type, IState> _states = new();
        public IState CurrentState { get; private set; }
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
            CurrentState?.OnUpdate();
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

            var prevType = CurrentState?.GetType();
            CurrentState?.OnExit();
            CurrentState = newState;
            CurrentState.OnEnter();

            OnStateChanged?.Invoke(prevType, newStateType);
        }

        public void Destroy()
        {
            foreach (var state in _states.Values) state.OnDestroy();
            _states.Clear();
            CurrentState = null;
        }
    }
}