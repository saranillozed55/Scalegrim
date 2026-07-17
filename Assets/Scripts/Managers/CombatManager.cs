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

        IsInCombat = true;
        
        //player cards move
        foreach (Lane lane in BoardLaneManager.Instance.LogicLanes)
        {
            if (lane.PlayerActiveCard != null && !lane.PlayerActiveCard._cardData.isDead)
            {
                await lane.PlayerActiveCard.PlayCardAttackAsync(Vector3.forward, lane.EnemyActiveCard); // Update parameter
            }
        }

        //enemy cards move
        foreach (Lane lane in BoardLaneManager.Instance.LogicLanes)
        {
            if (lane.EnemyActiveCard != null && !lane.EnemyActiveCard._cardData.isDead)
            {
                await lane.EnemyActiveCard.PlayCardAttackAsync(Vector3.back, lane.PlayerActiveCard);
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
