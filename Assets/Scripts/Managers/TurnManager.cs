using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : GenericSingleton<TurnManager>
{
    public TurnState CurrentTurnState { get; private set; }

    [Header("Broadcast to Event Channels")]
    [SerializeField] private VoidEventChannel _onPlayerStartTurn;

    [Header("Listener to Event Channels")]
    [SerializeField] private VoidEventChannel _onPlayerEndTurn;
    [SerializeField] private VoidEventChannel _onEnemyEndTurn;

    private Dictionary<TurnState, Action> turnStates;

    protected override void Awake()
    {
        base.Awake();
        turnStates = new Dictionary<TurnState, Action>
        {
            {TurnState.PlayerTurn, PlayerTurn },
            {TurnState.EnemyTurn, EnemyTurn },
        };
    }

    private void Start()
    {
        CurrentTurnState = TurnState.PlayerTurn;
    }

    private void OnEnable()
    {
        _onPlayerEndTurn.onEventRaised += SwitchTurnState;
        _onEnemyEndTurn.onEventRaised += SwitchTurnState;
    }

    private void OnDisable()
    {
        _onPlayerEndTurn.onEventRaised -= SwitchTurnState;
        _onEnemyEndTurn.onEventRaised -= SwitchTurnState;
    }

    private void PlayerTurn()
    {
        CinemachineSwitcher.Instance.SwitchState(CameraState.PlayerDeckCamera);
        //await probably in handmanager so we can wait until we have drawn card
        //this should be the only one sending out events
        Debug.Log("PlayerTurn");

    }

    private void EnemyTurn()
    {
        CinemachineSwitcher.Instance.SwitchState(CameraState.BoardCamera);

        _onPlayerStartTurn.RaiseEvent();
        Debug.Log("EnemyTurn");
    }

    private void SwitchTurnState() 
    {
        if(CurrentTurnState == TurnState.PlayerTurn)
        {
            SwitchTurnState(TurnState.EnemyTurn);
        }
        else
        {
            SwitchTurnState(TurnState.PlayerTurn);
        }
    }

    public void SwitchTurnState(TurnState newState)
    {
        CurrentTurnState = newState;

        if(!turnStates.TryGetValue(newState, out Action action))
        {
            return;
        }

        action();
    }
}
