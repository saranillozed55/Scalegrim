using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : GenericSingleton<InputManager>
{
    public Vector2 MouseScreenPosition { get; private set; }
    public bool LeftClickPressed { get; private set; }

    public event Action OnLeftClickPressed;
    public event Action OnRightClickPressed;

    private void OnPoint(InputValue value)
    {
        MouseScreenPosition = value.Get<Vector2>();
    }
    private void OnClick(InputValue value)
    {
        if (value.isPressed) OnLeftClickPressed?.Invoke();
    }
    private void OnRightClick(InputValue value)
    {
        if (value.isPressed) OnRightClickPressed.Invoke();
    }
}
