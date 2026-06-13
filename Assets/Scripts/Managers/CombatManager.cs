using UnityEngine;

public class CombatManager : GenericSingleton<CombatManager>
{
    [Header("Listener to Event Channel")]
    [SerializeField] private VoidEventChannel _onCombatStart;

    //[Header("Brodacast to Event Channels")]

    private CardAttackInvoker _invoker = new CardAttackInvoker();
    private void OnEnable()
    {
        _onCombatStart.onEventRaised += StartCombat;
    }

    private void OnDisable()
    {
        _onCombatStart.onEventRaised -= StartCombat;
    }

    private void StartCombat()
    {
        //queue up attack left to right
        Debug.Log("Cards should be attacking");


        foreach (Lane lane in BoardLaneManager.Instance.logicLanes)
        {
            if (lane.PlayerActiveCard != null)
            {
                _invoker.EnqueueCommand(new CardAttackCommand(lane.PlayerActiveCard, lane, () => ApplyCombatDamage(lane.PlayerActiveCard, lane))); // FIX THIS 
                //DON'T USE APPLY COMBAT DAMAGE, USE ANIMATION EVENTS IN UNITY ANIMATOR WINDOW
            }
            if (lane.EnemyActiveCard != null)
            {
                _invoker.EnqueueCommand(new CardAttackCommand(lane.EnemyActiveCard, lane, () => ApplyCombatDamage(lane.EnemyActiveCard, lane)));
            }
        }
    }

    //WRITE DOWN HOW THE COMBAT SYSTEM IS GOING TO WORK FIRST. PROBLEM SOLVE :). PLAYER SHOULD ATTACK FIRST THEN THE ENEMY THEN QUEUE UP CARDS
    private void ApplyCombatDamage(Card attacker, Lane lane)
    {
        if (attacker == null || lane == null) return;
        Debug.Log("Animation finished and is dealing damage in lane: " + lane.LaneIndex + " AI component: " + lane.EnemyActiveCard + " PlayerComponent: " + lane.PlayerActiveCard);
    }

    private void EndCombat()
    {

    }

    private void Update()
    {
        _invoker.Tick();
    }
}
