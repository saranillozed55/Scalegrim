using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using MainPlayer;
using System;
public class CombatManager : GenericSingleton<CombatManager>
{

    [Header("Listener to Event Channels")]
    [SerializeField] private VoidEventChannel _onCombatStart;

    private EncounterManager encounterManager;
    private EncounterData _encounterData;

    public int TurnsPassedThisCombat { get; private set; }

    public bool IsInCombat { get; private set; }

    public event Action OnCombatTurnEnded;

    private void Start()
    {
        encounterManager = FindFirstObjectByType<EncounterManager>();
        IsInCombat = false;

        /* Move these into LoadCombat Or something once we have UI stuff*/
        //Should choose from EncounterData, UpgradeSpotData, stuff like that rather than all in one script

        _encounterData = new EncounterData();
        BoardLaneManager.Instance.InitializeBoard(_encounterData);
    }

    private void OnEnable() 
    {
        _onCombatStart.onEventRaised += StartCombat;
    }

    private void OnDisable()
    {
        _onCombatStart.onEventRaised -= StartCombat;
    }

    //I BELIEVE START COMBAT SHOULD HAPPEN ONCE EVERYTHING HAS BEEN LOADED IN AND INITIALIZED SO USE ASYNC
    //^ meant to be in EncounterManager
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
        OnFinishCombatThisTurn();
    }

    public void ResetTurnsPassed()
    {
        TurnsPassedThisCombat = 0;
    }

    public void OnFinishCombatThisTurn()
    {
        TurnsPassedThisCombat++;
        IsInCombat = false;

        OnCombatTurnEnded?.Invoke();
        //_onEnemyEndTurn?.RaiseEvent();
    }
}
