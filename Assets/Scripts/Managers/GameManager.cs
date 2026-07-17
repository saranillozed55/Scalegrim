using UnityEngine;

public enum GameState { 
    Gameplay,
    //UI,
    Cutscene,
}


public class GameManager : GenericSingleton<GameManager>
{
    public GameState CurrentGameState { get; private set; }

    public bool onFocus = true;

    //private void Start()
    //{
    //    Cursor.lockState = CursorLockMode.Locked;
    //}
    private void Update()
    {
        Cursor.lockState = onFocus ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void OnEnable()
    {
        InputManager.Instance.OnPauseButtonPressed += OnGameStatePaused;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnPauseButtonPressed -= OnGameStatePaused;
    }

    //Link pause and play together later
    public void OnGameStatePaused()
    {
        Time.timeScale = 0;
    }
    public void OnGameStatePlay()
    {
        Time.timeScale = 1;
    }

    public void SwitchGameState(GameState newState)
    {
        CurrentGameState = newState;
    }
}
