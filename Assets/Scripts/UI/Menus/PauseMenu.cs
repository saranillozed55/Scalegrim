using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PauseMenu : BaseUI
{
    private Button _backToGameButton;
    private Button _optionsButton;
    private Button _quitButton;

    [Header("Listener to Event Channels")]
    [SerializeField] private VoidEventChannel _initialPauseMenu;

    private void OnEnable()
    {
        _initialPauseMenu.onEventRaised += PushToStack;
    }

    private void OnDisable()
    {
        if (_backToGameButton != null)
        {
            _backToGameButton.UnregisterCallback<ClickEvent>(OnBackToGameButtonClicked);
        }
        if (_optionsButton != null)
        {
            _optionsButton.UnregisterCallback<ClickEvent>(OnOptionsButtonClicked);
        }
        if (_quitButton != null)
        {
            _quitButton.UnregisterCallback<ClickEvent>(OnQuitButtonClicked);
        }

        _initialPauseMenu.onEventRaised -= PushToStack;
    }

    public override void OnOpen()
    {
        base.OnOpen();

        _backToGameButton = Container.Q<Button>("BackToGameButton");
        _optionsButton = Container.Q<Button>("OptionsButton");
        _quitButton = Container.Q<Button>("QuitButton");

        if (_backToGameButton == null || _optionsButton == null || _quitButton == null)
        {
            Debug.LogError("Pause menu button has null reference");
        }
        else
        {
            _backToGameButton.RegisterCallback<ClickEvent>(OnBackToGameButtonClicked);
            _optionsButton.RegisterCallback<ClickEvent>(OnOptionsButtonClicked);
            _quitButton.RegisterCallback<ClickEvent>(OnQuitButtonClicked);
        }
    }
    public override void OnClose()
    {
        base.OnClose();
        _backToGameButton.UnregisterCallback<ClickEvent>(OnBackToGameButtonClicked);
        _optionsButton.UnregisterCallback<ClickEvent>(OnOptionsButtonClicked);
        _quitButton.UnregisterCallback<ClickEvent>(OnQuitButtonClicked);
    }
    private void OnBackToGameButtonClicked(ClickEvent evt)
    {
        UIManager.Instance.Pop(this);

        InputManager.Instance.SwitchState(InputState.Player);
        GameManager.Instance.SwitchGameState(GameState.Gameplay);
        Debug.Log("Back to game button was pressed");
    }
    private void OnOptionsButtonClicked(ClickEvent evt)
    {
        InputManager.Instance.SwitchState(InputState.UI);
        GameManager.Instance.SwitchGameState(GameState.UI);

        UI.Events.UIEventBus.RaiseOnOptionsButtonClicked();

        Debug.Log("Options button was pressed");
    }

    private void OnQuitButtonClicked(ClickEvent evt)
    {
        // Logs the exit attempt in the console
        Debug.Log("Player has quit the game.");

#if UNITY_EDITOR
        // Stops playmode if you are testing inside the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // Closes the application if it is a standalone build
            Application.Quit();
#endif
    }
}
