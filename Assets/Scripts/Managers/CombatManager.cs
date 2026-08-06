using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : GenericSingleton<CombatManager>
{
    [Header("Broadcast to Event Channels")]
    [SerializeField] private VoidEventChannel _onEnemyEndTurn;

    [Header("Listener to Event Channels")]
    [SerializeField] private VoidEventChannel _onCombatStart;

    public bool IsInCombat { get; private set; }

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
        List<Lane> lanesSanpShot = new List<Lane>(BoardLaneManager.Instance.LogicLanes);
        foreach (Lane lane in lanesSanpShot)
        {
            if (lane.PlayerActiveCard != null && !lane.PlayerActiveCard.Dead)
            {
                CardView view = CardView.GetView(lane.PlayerActiveCard);
                if (view != null) await view.CardAttackAsync(lane.PlayerActiveCard, lane.EnemyActiveCard);
            }
        }

        //enemy cards move
        foreach (Lane lane in lanesSanpShot)
        {
            if (lane.EnemyActiveCard != null && !lane.EnemyActiveCard.Dead)
            {
                CardView view = CardView.GetView(lane.EnemyActiveCard);
                if (view != null) await view.CardAttackAsync(lane.EnemyActiveCard, lane.PlayerActiveCard);

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
