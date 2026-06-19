using System;
using System.Collections.Generic;
using LitFramework.FSM;


public class ProcedureManager
{
    private StateMachine _fsm;  // 复用你已有的状态机
    private Dictionary<Type, ProcedureBase> _procedures = new();
    public ProcedureManager(StateMachine fsm)
    {
        _fsm = fsm;
    }

    public ProcedureBase CurrentProcedure { get; private set; }

    public void RegisterProcedure<T>(T procedure) where T : ProcedureBase
    {
        _procedures[typeof(T)] = procedure;
        _fsm.RegisterState(procedure);  // 内部调用 state.OnInit
    }

    public void ChangeProcedure<T>() where T : ProcedureBase
    {
        _fsm.ChangeState<T>();
    }

    public void Update() => _fsm.Update();

    // 流程专用的方法
    public T GetProcedure<T>() where T : ProcedureBase => _procedures[typeof(T)] as T;

    public T GetSharedData<T>(string key) => (T)CurrentProcedure?.SharedData[key];
    public void SetSharedData(string key, object value) => CurrentProcedure?.SharedData.Add(key, value);
}