using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputService : IInputService, IDisposable
{
    private GameControls _controls;
    public InputService()
    {
        _controls = new GameControls();
        _controls.Gameplay.Enable();
    }

    public void Dispose()
    {
        if (_controls != null)
        {
            _controls.Gameplay.Disable();
            _controls.Dispose();   // 释放整个 InputActionAsset 资源
            _controls = null;
        }
    }

    public Vector2 GetMoveInput() => _controls.Gameplay.Move.ReadValue<Vector2>();
}
