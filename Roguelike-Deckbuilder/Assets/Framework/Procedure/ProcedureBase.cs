using System.Collections;
using System.Collections.Generic;
using LitFramework.FSM;
using UnityEngine;

public abstract class ProcedureBase : IState
{
    // 流程特有的扩展：数据共享、取消令牌等
    public Dictionary<string, object> SharedData { get; } = new();

    public abstract void OnDestroy();

    public abstract void OnEnter();

    public abstract void OnExit();

    public abstract void OnInit();

    public abstract void OnUpdate();

}