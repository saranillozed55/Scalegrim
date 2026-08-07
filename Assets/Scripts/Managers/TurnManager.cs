using UnityEngine;

public class TurnManager : GenericSingleton<TurnManager>
{
    public TurnState CurrentTurnState { get; private set; }

    [Header("Broadcast to Event Channels")]
    [SerializeField] private CameraStateEventChannel _onEndTurnCam; // don't use this anymore


    [Header("Listener to Event Channels")]
    [SerializeField] private VoidEventChannel _onPlayerEndTurn;
    [SerializeField] private VoidEventChannel _onEnemyEndTurn;
    [SerializeField] private VoidEventChannel _onPlayerFinishedDrawCard;


    private void Start()
    {
        CurrentTurnState = TurnState.PlayerTurn;
    }

    private void OnEnable()
    {
        _onPlayerEndTurn.onEventRaised += SwitchTurnState;
        _onEnemyEndTurn.onEventRaised += SwitchTurnState;
        _onPlayerFinishedDrawCard.onEventRaised += SwitchTurnState;
    }
    private void OnDisable()
    {
        _onPlayerEndTurn.onEventRaised -= SwitchTurnState;
        _onEnemyEndTurn.onEventRaised -= SwitchTurnState;
        _onPlayerFinishedDrawCard.onEventRaised -= SwitchTurnState;
    }

    private void SwitchTurnState() 
    {
        if(CurrentTurnState == TurnState.PlayerTurn)
        {
            CurrentTurnState = TurnState.EnemyTurn;
            _onEndTurnCam.RaiseEvent(CameraState.BoardCamera);
        }
        else
        {
            CurrentTurnState = TurnState.PlayerMustDraw;
            _onEndTurnCam.RaiseEvent(CameraState.PlayerDeckCamera);
        }
    }
}
