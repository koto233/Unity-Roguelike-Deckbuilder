using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ModelContainer
{
    private static Dictionary<Type, IModel> _models = new Dictionary<Type, IModel>();
    
    // 注册模型（通常在 GameRoot 或启动时调用一次）
    public static void Register<T>(T model) where T : IModel
    {
        var type = typeof(T);
        if (_models.ContainsKey(type))
        {
            Debug.LogWarning($"Model {type.Name} already registered, overwriting.");
            _models[type] = model;
        }
        else
        {
            _models.Add(type, model);
        }
        model.OnRegister();
    }
    
    // 获取模型
    public static T Get<T>() where T : IModel
    {
        if (_models.TryGetValue(typeof(T), out var model))
            return (T)model;
        throw new Exception($"Model {typeof(T).Name} not registered.");
    }
    
    // // 可选：新游戏时重置所有模型（重新创建实例）
    // public static void ResetAll()
    // {
    //     // 重新创建每个模型的实例（需要工厂或手动）
    //     // 简单起见，也可以直接重新注册新实例
    //     var oldModels = _models.Values.ToList();
    //     _models.Clear();
    //     foreach (var model in oldModels)
    //     {
    //         // 如果模型有 Reset 方法，可以调用；否则重新实例化需要知道具体类型
    //         // 这里用简单方式：要求模型实现 IResetable
    //         if (model is IResetable resetable)
    //             resetable.Reset();
    //         else
    //             Debug.LogError($"Model {model.GetType().Name} cannot reset automatically.");
    //     }
    // }
}

public interface IResetable
{
    void Reset();
}