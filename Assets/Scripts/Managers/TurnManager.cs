using UnityEngine;

public class TurnManager : GenericSingleton<TurnManager>
{

    public TurnState CurrentTurnState { get; private set; }
    [Header("Broadcast to Event Channels")]
    [SerializeField] private CameraStateEventChannel _onEndTurnCam;


    [Header("Listener to Event Channels")]
    [SerializeField] private VoidEventChannel _onEndTurn;
    [SerializeField] private VoidEventChannel _onEnemyEndTurn;


    private void Start()
    {
        CurrentTurnState = TurnState.PlayerTurn;
    }

    private void OnEnable()
    {
        _onEndTurn.onEventRaised += SwitchTurnState;
    }
    private void OnDisable()
    {
        _onEndTurn.onEventRaised -= SwitchTurnState;
    }

    private void SwitchTurnState() 
    {
        if(CurrentTurnState == TurnState.PlayerTurn)
        {
            CurrentTurnState = TurnState.EnemyTurn;
            _onEndTurnCam.RaiseEvent(CameraState.BoardCamera);
            Debug.Log("Switch state to: " + CurrentTurnState);
        }
        else
        {
            CurrentTurnState = TurnState.PlayerTurn;
            _onEndTurnCam.RaiseEvent(CameraState.FPCamera);
            Debug.Log("Switch state to: " + CurrentTurnState);
        }
    }
}
