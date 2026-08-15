using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : GenericSingleton<UIManager>
{
    private Stack<IUIToolkit> _uiStack = new();

    [Header("Broadcast to Event Channels")]
    [SerializeField] private VoidEventChannel _initialPauseMenu;

    private void Start()
    {
        SubscribeToEvents();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        UnsubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        InputManager.Instance.OnPauseButtonPressed += OnPauseRaised;
    }

    private void UnsubscribeToEvents()
    {
        if(InputManager.Instance != null)
            InputManager.Instance.OnPauseButtonPressed -= OnPauseRaised;
    }

    private void OnPauseRaised()
    {
        if (_uiStack.Count > 0) // move this
        {
            Pop(_uiStack.Peek());
            if(_uiStack.Count == 0)
            {
                InputManager.Instance.SwitchState(InputState.Player);
                GameManager.Instance.OnGameStatePlay();
            }
            return;
        }

        _initialPauseMenu.RaiseEvent();
        InputManager.Instance.SwitchState(InputState.UI);
    }

    public void Push(IUIToolkit ui)
    {
        if (_uiStack.Count > 0)
        {
            _uiStack.Peek().OnLoseFocus(); // not sure if we use lose focus here, rather might just change the sort order
        }

        _uiStack.Push(ui);
        Debug.Log("Pushed: " + ui);
        ui.OnOpen();
    }

    public void Pop(IUIToolkit ui)
    {
        if (_uiStack.Count == 0) return;
        if (_uiStack.Peek() != ui)
        {
            Debug.LogWarning($"UIManager.Pop called for a non-top UI ({ui}). Current top: {_uiStack.Peek()}");
            return;
        }
        _uiStack.Pop().OnClose();

        if (_uiStack.Count > 0) _uiStack.Peek().OnFocus();
    }
    public void PopAll()
    {
        while (_uiStack.Count > 0)
        {
            _uiStack.Pop().OnClose();
        }
    }
}
