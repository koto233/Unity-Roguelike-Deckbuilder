using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class GameOverView : UIWindow
{
    public event Action OnRestart;
    public event Action OnQuit;

    void OnEnable()
    {
        b_Restart.onClick.AddListener(() =>
        {
            OnRestart?.Invoke();
        });
        b_Quit.onClick.AddListener(() =>
        {
            OnQuit?.Invoke();
        });
    }
    void OnDisable()
    {
        b_Restart.onClick.RemoveAllListeners();
        b_Quit.onClick.RemoveAllListeners();
    }
}