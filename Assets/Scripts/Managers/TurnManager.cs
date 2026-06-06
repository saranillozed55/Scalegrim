using UnityEngine;

public class TurnManager : GenericSingleton<TurnManager>
{

    public TurnState CurrentTurnState { get; private set; }

    private void Start()
    {
        CurrentTurnState = TurnState.PlayerTurn;
    }

}
