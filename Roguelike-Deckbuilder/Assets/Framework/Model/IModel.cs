using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModel
{
    void OnRegister();   // 注册后回调，可用于加载存档或初始化
}