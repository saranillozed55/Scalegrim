using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum GameState { 
    Gameplay,
    UI,
    Cutscene,
}


public class GameManager : GenericSingleton<GameManager>
{
    public GameState CurrentGameState { get; private set; }

    public bool onFocus = true;

    private Dictionary<GameState, Action> gameStateActions;

    protected override void Awake()
    {
        base.Awake();
        InitGameStateActions();
    }

    private void InitGameStateActions()
    {
        gameStateActions = new Dictionary<GameState, Action>
        {
            {GameState.Gameplay, OnGameStatePlay },
            {GameState.UI, OnGameStatePaused},
        };
    }

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
        Debug.Log("OnGameStatePaused");
    }
    public void OnGameStatePlay()
    {
        Time.timeScale = 1;
        Debug.Log("OnGameStatePlay");
    }

    public void SwitchGameState(GameState newState) // should be using this method to switch game states instead of directly changing the CurrentGameState variable
    {
        CurrentGameState = newState;
        
        if(!gameStateActions.TryGetValue(newState, out Action action))
        {
            return;
        }

        action();
    }
}
