using UnityEngine;

public class CombatManager : GenericSingleton<CombatManager>
{
    [Header("Broadcast to Event Channels")]
    [SerializeField] private VoidEventChannel _onEnemyEndTurn;


    [Header("Listener to Event Channels")]
    [SerializeField] private VoidEventChannel _onCombatStart;


    public bool IsInCombat { get; private set; }

    //[Header("Brodacast to Event Channels")]

    //private CardAttackInvoker _invoker = new CardAttackInvoker();

    private void Start()
    {
        IsInCombat = false;
    }

    private void OnEnable()
    {
        _onCombatStart.onEventRaised += StartCombat;
    }

    private void OnDisable()
    {
        _onCombatStart.onEventRaised -= StartCombat;
    }

    private async void StartCombat()
    {
        if (IsInCombat) return;

        Debug.Log("Cards should be attacking");

        IsInCombat = true;
        //player cards move
        foreach (Lane lane in BoardLaneManager.Instance.logicLanes)
        {
            if (lane.PlayerActiveCard != null)
            {
                await lane.PlayerActiveCard.PlayCardAttackAsync(Vector3.forward, lane.EnemyActiveCard); // Update parameter
                //_invoker.EnqueueCommand(new CardAttackCommand(lane.PlayerActiveCard, lane)); 
            }
        }

        //enemy cards move
        foreach (Lane lane in BoardLaneManager.Instance.logicLanes)
        {
            if (lane.EnemyActiveCard != null)
            {
                await lane.EnemyActiveCard.PlayCardAttackAsync(Vector3.back, lane.PlayerActiveCard);
                //_invoker.EnqueueCommand(new CardAttackCommand(lane.EnemyActiveCard, lane));
            }
        }

        EndCombat();
    }
    public void EndCombat()
    {
        IsInCombat = false;
        _onEnemyEndTurn.RaiseEvent();
    }
}
