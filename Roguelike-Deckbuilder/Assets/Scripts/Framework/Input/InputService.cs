using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputService : IInputService
{
    private GameControls _controls;
    public InputService()
    {
        _controls = new GameControls();
        _controls.Gameplay.Enable();
    }

    public Vector2 GetMoveInput() => _controls.Gameplay.Move.ReadValue<Vector2>();
}
