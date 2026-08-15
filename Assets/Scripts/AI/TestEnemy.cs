using Cards.Events;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class TestEnemy : MonoBehaviour, IDamageable
{
    [Header("Blueprint list")]
    [SerializeField] private List<Blueprint> _bluePrints; // each enemy should have a list of blueprints, 
    // but for testing purposes we will have a list of blueprints in this script
    //I believe after reaching a different point in the game, the enemy will get different blueprint lists 

    [Header("Listener to Event Channels")]
    [SerializeField] private VoidEventChannel _onEnemyEndTurn;

    //This specific broadcast should be use for all enemies since all enemies will have the same logic, 
    //except for bosses

    [Header("Broadcast to Event Channels")]
    [SerializeField] private VoidEventChannel _onSurrenderPerformed;

    [Header("AI profile")]
    [SerializeField] private AIPersonality _aiProfile;

    //BoardEvaluater eval = new BoardEvaluater();
    CardPlacer cardPlacer = new CardPlacer();
    EnemyTurnQueue enemyTurnQueue = new EnemyTurnQueue();
    CardRetriever cardRetriever;

    [Header("Testing references")]
    [SerializeField] private BlueprintRetriever blueprintRetriever;
    [SerializeField] private CardGroupRetriever cardGroupRetriever;
    [SerializeField] private int Health = 15;

    private void OnEnable()
    {
        //_onEnemyEndTurn.onEventRaised += QueueNextCardInLane;
        CardEventBus.OnDirectEnemyDamage += TakeDamage;
        _onEnemyEndTurn.onEventRaised += OnEnemyEndTurnHandler;
    }
    private void OnDisable()
    {
        //_onEnemyEndTurn.onEventRaised -= QueueNextCardInLane;
        CardEventBus.OnDirectEnemyDamage -= TakeDamage;
        _onEnemyEndTurn.onEventRaised -= OnEnemyEndTurnHandler;
    }

    private void Start()
    {
        blueprintRetriever.SetBlueprintsByDifficulty(_bluePrints);
        OnCombatStart();
        OnEnemyEndTurnHandler();
    }

    private BoardState CheckCurrentBoardState()
    {
        BoardState currState = BoardLaneManager.Instance.CaptureBoardState();

        for (int i = 0; i < currState.LanesShot.Count; i++)
        {
            Debug.Log($" LaneIndex {i + 1}, EnemyQueuedCard: {currState.LanesShot[i].EnemyQueuedCard}, EnemyCard: {currState.LanesShot[i].EnemyCard},  PlayerCard: {currState.LanesShot[i].PlayerCard}");
        }
        return currState;
    }

    private int ChooseDifficultyLevel()
    {
        //this should be based on the player's performance and how far they have progressed in the game
        return 1; //currently testing
    }

    private int GetDifficultyLevel()
    {
        return 1;
    }

    private void OnCombatStart()
    {
        if (_bluePrints.Count <= 0)
        {
            Debug.Log("No blueprints in enemy list");

            return;
        }

        Blueprint blueprint = blueprintRetriever.GetBlueprintByDifficultyAndRandom(GetDifficultyLevel());

        //HERE: SHOULD GET THE BLUEPRINT FOR ENEMIES USING THEIR ID THEN WE CAN GRAB THE SPECIFIC BLUEPRINT AND RANDOMIZE

        Debug.Log($"Blueprint chosen: {blueprint.name}, Difficulty: {blueprint.difficultyLevelOfBlueprint}");
        enemyTurnQueue.GenerateEnemyQueue(blueprint);
    }

    private async void OnEnemyEndTurnHandler()
    {
        // call the async Task<bool> method and ignore the returned value
        await OpponentTurnPerformed(); // result is ignored but can handle if needed, not much different from _ = 
    }

    //this won't be in this script this is still a test script, but this is the main logic for the AI to decide what to do on its turn
    private async Task<bool> OpponentTurnPerformed()
    {
        BlueprintTurn blueprint = enemyTurnQueue.GetTurnBlueprint();

        if (blueprint == null)
        {
            Debug.Log("Enemy will have to surrender, no more turns");
            _onSurrenderPerformed?.RaiseEvent();
            return false;
        }

        List<int> availableLanes = BoardLaneManager.Instance.GetAvailableEnemyLanes();

        foreach (BlueprintEntry turn in blueprint.Entries)
        {
            EntryType type = turn.type;
            EnemyAttackPreference preference = turn.enemyAttackPreference;

            int randomIndex = Random.Range(0, availableLanes.Count);
            int lane = availableLanes[randomIndex];

            bool wasPlaced = false;

            if (type == EntryType.ExactCard)
            {
                //Preference here?

                Debug.Log($"Type: {type}, Card Name: {turn.card.name}, Card Group: {turn.card.Group}");

                CardModel model = new CardModel(turn.card);
                wasPlaced = await cardPlacer.HandlePlaceCard(model, lane);
            }
            else if (type == EntryType.RandomGroup)
            {

                Debug.Log($"Type: {type}, Group {turn.group}- random card from group will be taken");

                // implement GetCardFromGroup method in CardRetriever class to get a random card from the group 
                List<CardDataSO> cards = cardGroupRetriever.GetCardsByGroup(turn.group);
                CardDataSO randomCard = cards[Random.Range(0, cards.Count)];
                CardModel model = new CardModel(randomCard);

                wasPlaced = await cardPlacer.HandlePlaceCard(model, lane);
            }
            else if (type == EntryType.RandomFromAny)
            {
                if (turn.cardListFromAny == null || turn.cardListFromAny.Count == 0)
                {
                    Debug.LogWarning($"Blueprint entry: {turn} has an empty card list");
                    continue;
                }

                List<CardDataSO> cards = cardGroupRetriever.GetCardsFromAnyInList(turn.cardListFromAny);
                CardDataSO randomCard = cards[Random.Range(0, cards.Count)];
                CardModel model = new CardModel(randomCard);

                wasPlaced = await cardPlacer.HandlePlaceCard(model, lane); // REMOVE _enemyDeck from this method
            }


            if(wasPlaced)
            {
                availableLanes.RemoveAt(randomIndex); // remove the lane from available lanes if the card was placed successfully
                Debug.Log($"Card was sucessfully placed in lane {lane + 1}");
            }
        }
        return true;
    }
    public void TakeDamage(int val)
    {
        Health -= val;
        if (Health <= 0)
        {

        }
        Debug.Log($"TestEnemy Health: {Health}");
    }

    //private void QueueNextCardInLane()
    //{
    //    List<LaneSnapShot> lanes = CheckCurrentBoardState().Lanes;

    //    foreach (LaneSnapShot lane in lanes) // O(n) since only have max 4 lanes
    //    {
    //        //float score = EvaluateLane(lane);
    //        float score = eval.EvaluateLane(_aiProfile, lane);

    //        if (score >= 15)
    //        {
    //            Debug.Log($"<color=yellow> Lane {lane.LaneIndex + 1} has a score of {score} >= 15. Queuing strongest card available in this lane.</color>");

    //            CardModel currentStrongestCard = null;
    //            CardModel retrievedCard = cardRetriever.RetrieveCard(_aiProfile, lane);
    //            currentStrongestCard = retrievedCard;

    //            if (currentStrongestCard != null)
    //            {
    //                _ = cardPlacer.HandlePlaceCard(currentStrongestCard, lane.LaneIndex);
    //            }
    //            continue;
    //        }

    //        else if (score > DAMAGE_THRESHOLD)
    //        {
    //            Debug.Log($"<color=yellow>Lane {lane.LaneIndex + 1} has a score of {score}. Queueing next card in this lane.</color>");

    //            foreach (CardModel card in _enemyDeck)
    //            {
    //                if (card == null)
    //                {
    //                    Debug.LogWarning($"Card that is being accessed is null");
    //                    continue;
    //                }
    //                if (lane.PlayerCard.HasValue && (card.Health > lane.PlayerCard.Value.Attack))
    //                {
    //                    Debug.Log($"<color=cyan> This Card '{card.Name}' has enough health to survive the player's attack. Queueing this card in lane {lane.LaneIndex + 1}.</color>");

    //                    _ = cardPlacer.HandlePlaceCard(card, lane.LaneIndex);
    //                    break;
    //                }
    //                else if (!lane.PlayerCard.HasValue)
    //                {
    //                    Debug.Log($"<color=cyan> There is no Player Card in this lane {lane.LaneIndex + 1}.</color>");
    //                    break;
    //                }
    //                else
    //                {
    //                    Debug.Log($"<color=cyan> This card '{card.Name}' can't survive player attack");
    //                }
    //            }
    //        }i

    //        else
    //        {
    //            Debug.Log($"<color=white>Lane {lane.LaneIndex + 1} has a score of {score}. Not queueing next card in this lane.</color>");
    //        }
    //    }
    //}
}
