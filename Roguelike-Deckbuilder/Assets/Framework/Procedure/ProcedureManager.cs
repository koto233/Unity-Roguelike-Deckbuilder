using System;
using System.Collections.Generic;
using LitFramework.FSM;
using UnityEngine;

public class ProcedureManager
{
    private StateMachine _fsm;
    private Dictionary<Type, ProcedureBase> _procedures = new();
    private Dictionary<Type, object> _pendingArgs = new(); // 临时参数缓存

    public ProcedureBase CurrentProcedure { get; private set; }

    public ProcedureManager(StateMachine fsm)
    {
        _fsm = fsm;
    }

    public void RegisterProcedure<T>(T procedure) where T : ProcedureBase
    {
        _procedures[typeof(T)] = procedure;
        _fsm.RegisterState(procedure);
    }

    // 无参切换（原有方法）
    public void ChangeProcedure<T>() where T : ProcedureBase
    {
        CurrentProcedure = _procedures[typeof(T)];
        _fsm.ChangeState<T>();
    }

    // ★ 带参切换（新增）
    public void ChangeProcedure<T, TArgs>(TArgs args) where T : ProcedureBase where TArgs : IProcedureArgs
    {
        // 1. 以目标状态类型为Key存储参数
        _pendingArgs[typeof(T)] = args;

        // 2. 更新当前流程实例（必须，否则 GetArgs 拿不到类型）
        CurrentProcedure = _procedures[typeof(T)];

        // 3. 切换状态（底层状态机将调用目标状态的 OnEnter）
        _fsm.ChangeState<T>();
    }

    // ★ 目标状态在 OnEnter 中调用此方法获取参数
    public TArgs GetArgs<TArgs>()
    {
        if (CurrentProcedure == null)
        {
            Debug.LogError("CurrentProcedure 为空，无法获取参数");
            return default;
        }

        var key = CurrentProcedure.GetType(); // 获取实际类型
        if (_pendingArgs.TryGetValue(key, out var args))
        {
            _pendingArgs.Remove(key); // 取出即销毁，防残留
            return (TArgs)args;
        }

        Debug.LogWarning($"未找到类型 {key.Name} 的临时参数");
        return default;
    }

    // 可选：更安全的 TryGet 版本
    public bool TryGetArgs<TArgs>(out TArgs args)
    {
        args = default;
        if (CurrentProcedure == null) return false;

        var key = CurrentProcedure.GetType();
        if (_pendingArgs.TryGetValue(key, out var raw))
        {
            _pendingArgs.Remove(key);
            args = (TArgs)raw;
            return true;
        }
        return false;
    }

    public void Update() => _fsm.Update();

    public T GetProcedure<T>() where T : ProcedureBase => _procedures[typeof(T)] as T;

    // 共享数据（保留原功能）
    public T GetSharedData<T>(string key) => (T)CurrentProcedure?.SharedData[key];
    public void SetSharedData(string key, object value) => CurrentProcedure?.SharedData.Add(key, value);
}