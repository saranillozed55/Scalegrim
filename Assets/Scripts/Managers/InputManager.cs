using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public enum InputState
{
    Player,
    UI,
}

public class InputManager : GenericSingleton<InputManager>
{
    private PlayerInput _playerInput;
    private InputState _inputState;
    public Vector2 MouseScreenPosition { get; private set; }
    public bool LeftClickPressed { get; private set; }

    public event Action OnLeftClickPressed;
    public event Action OnRightClickPressed;
    public event Action OnForwardButtonPressed;
    public event Action OnBackButtonPressed; //Called cancel in action map - this is for when we click a card but then want to go back to our hand
    public event Action OnPauseButtonPressed; 

    private string PlayerActionMapString = "Player";
    private string UIActionMapString = "UI";

    [SerializeField] private UIDocument _document; // this is for checking if pause menu is up so that raycasts don't go through

    private Dictionary<InputState, InputActionMap> _actionMaps;
    private InputActionMap _currentMap;

    protected override void Awake()
    {
        base.Awake();
        _playerInput = GetComponent<PlayerInput>();
        InitalizeInputStates();
    }

    private void Start()
    {
        _playerInput.SwitchCurrentActionMap(PlayerActionMapString);
    }

    public bool IsPointerOverUI
    {
        get 
        {
            if (_document == null) return false;
            IPanel panel = _document.rootVisualElement.panel;
            Vector2 panelPos = RuntimePanelUtils.ScreenToPanel(panel, MouseScreenPosition);

            VisualElement pickedElement = panel.Pick(panelPos);
            return pickedElement != null;
        }
    }

    private void InitalizeInputStates()
    {
        var actionsAsset = _playerInput.actions;
        _actionMaps = new Dictionary<InputState, InputActionMap>
        {
            {InputState.Player, actionsAsset.FindActionMap(PlayerActionMapString)},
            {InputState.UI, actionsAsset.FindActionMap(UIActionMapString)}
        };
    }
    #region Mouse/Keyboard Inputs
    private void OnPoint(InputValue value)
    {
        MouseScreenPosition = value.Get<Vector2>();
    }
    private void OnClick(InputValue value) // must match exactly as the action map. Can add or remove parameter if we want to perform any logic with the value
    {
        if (value.isPressed) OnLeftClickPressed?.Invoke();
    }
    private void OnRightClick(InputValue value)
    {
        if (value.isPressed) OnRightClickPressed?.Invoke();
    }
    private void OnBack(InputValue value)
    {
        if (value.isPressed) OnBackButtonPressed?.Invoke();
    }

    private void OnForward(InputValue value)
    {
        if (value.isPressed) OnForwardButtonPressed?.Invoke();
    }
    private void OnPause(InputValue value)
    {
        Debug.Log("On Pause was pressed");
        if (value.isPressed)
        {
            OnPauseButtonPressed?.Invoke();
        }
    }
    #endregion
    
    public void SwitchState(InputState newState)
    {
        if(!_actionMaps.TryGetValue(newState, out InputActionMap targetMap))
        {
            Debug.LogWarning($"[InputManager] No map registered for state: {newState}");
            return;
        }

        //avoid invocation frames if already running this map
        if (_currentMap == targetMap) return;

        //safety shutdown of active maps to block input leak
        _playerInput.currentActionMap?.Disable();

        _currentMap = targetMap;
        _currentMap.Enable();

        _playerInput.SwitchCurrentActionMap(_currentMap.name);
        Debug.Log($"[InputManager] Successfully swapped map focus to: {newState}");
    }
}
