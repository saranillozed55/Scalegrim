using UnityEngine;

public class EndTurn : MonoBehaviour, IClickable
{
    [Header("Broadcast to Event Channels")]
    [SerializeField] private VoidEventChannel _onPlayerEndTurn;

    [Header("Settings")]
    [SerializeField] private float _onEndTurnSpeed = .5f;

    private bool clicked = false;


    public async void OnClick() // should probably not be async void
    {
        if (clicked || TurnManager.Instance.CurrentTurnState != TurnState.PlayerTurn) // this needs to check if its even player turn or not
        {
            return;
        }

        clicked = true; // not using this yet
        await Awaitable.WaitForSecondsAsync(_onEndTurnSpeed);
        _onPlayerEndTurn.RaiseEvent();
        clicked = false;
    }
}
