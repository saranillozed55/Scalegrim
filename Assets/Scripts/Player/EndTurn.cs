using UnityEngine;

public class EndTurn : MonoBehaviour, IClickable
{
    [Header("Broadcast to Event Channels")]
    [SerializeField] private VoidEventChannel _onEndTurn;

    [Header("Settings")]
    [SerializeField] private float _onEndTurnSpeed = .5f;

    private bool clicked = false;


    public async void OnClick()
    {
        await Awaitable.WaitForSecondsAsync(_onEndTurnSpeed);
        _onEndTurn.RaiseEvent();
    }
}
