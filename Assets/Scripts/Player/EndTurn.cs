using UnityEngine;

public class EndTurn : MonoBehaviour, IClickable
{
    [Header("Broadcast to Event Channels")]
    [SerializeField] private VoidEventChannel _onEndTurn;

    public void OnClick()
    {
        _onEndTurn.RaiseEvent();
    }
}
