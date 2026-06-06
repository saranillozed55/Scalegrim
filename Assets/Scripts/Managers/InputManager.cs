using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : GenericSingleton<InputManager>
{
    public Vector2 MouseScreenPosition { get; private set; }
    public bool LeftClickPressed { get; private set; }

    public event Action OnLeftClickPressed;
    public event Action OnRightClickPressed;
    public event Action OnBackButtonPressed;

    private void OnPoint(InputValue value)
    {
        MouseScreenPosition = value.Get<Vector2>();
    }
    private void OnClick(InputValue value) // must match exactly as the action map
    {
        if (value.isPressed) OnLeftClickPressed?.Invoke();
    }
    private void OnRightClick(InputValue value)
    {
        if (value.isPressed) OnRightClickPressed?.Invoke();
    }
    private void OnCancel(InputValue value)
    {
        if(value.isPressed) OnBackButtonPressed?.Invoke();
    }
}
