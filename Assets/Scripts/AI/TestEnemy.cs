using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public enum CardPreference
{
    OVERPOWER,
    TANK,
}

public class TestEnemy : MonoBehaviour
{
    [SerializeField] private List<CardDataSO> _enemyCards;

    [SerializeField] private List<CardModel> _enemyDeck = new();

    [Header("Blueprint list")]
    [SerializeField] private List<Blueprint> _bluePrints;

    [Header("Listener to Event Channels")]
    [SerializeField] private VoidEventChannel _onEnemyEndTurn;

    [Header("Settings for Next Card Moves")]
    [SerializeField] private float DAMAGE_THRESHOLD = 5f;

    [Header("AI profile")]
    [SerializeField] private AIPersonality _aiProfile;

    BoardEvaluater eval = new BoardEvaluater();
    CardPlacer cardPlacer = new CardPlacer();
    EnemyTurnQueue enemyTurnQueue = new EnemyTurnQueue();
    CardRetriever cardRetriever;

    private int turnsPassed = 0;

    private void OnEnable()
    {
        //_onEnemyEndTurn.onEventRaised += QueueNextCardInLane;
        _onEnemyEndTurn.onEventRaised += OnEnemyEndTurnHandler;
    }

    private void OnDisable()
    {
        //_onEnemyEndTurn.onEventRaised -= QueueNextCardInLane;
        _onEnemyEndTurn.onEventRaised -= OnEnemyEndTurnHandler;
    }

    private void Start()
    {
        InitBaseDeck();
        OnCombatStart();
        cardRetriever = new CardRetriever(_enemyDeck);

        _ = cardPlacer.HandlePlaceCard(_enemyDeck, _enemyDeck[0], 0);
        _ = cardPlacer.HandlePlaceCard(_enemyDeck, _enemyDeck[1], 1);
    }

    private void InitBaseDeck() // won't be using this anymore because will have blueprints later on
    {
        for (int i = 0; i < _enemyCards.Count; i++)
        {
            _enemyDeck.Add(new CardModel(_enemyCards[i]));
        }
    }

    private BoardState CheckCurrentBoardState()
    {
        BoardState currState = BoardLaneManager.Instance.CaptureBoardState();

        for (int i = 0; i < currState.Lanes.Count; i++)
        {
            Debug.Log($" LaneIndex {i + 1}, EnemyQueuedCard: {currState.Lanes[i].EnemyQueuedCard}, EnemyCard: {currState.Lanes[i].EnemyCard},  PlayerCard: {currState.Lanes[i].PlayerCard}");
        }
        return currState;
    }

    private void OnCombatStart()
    {
        if (_bluePrints.Count <= 0)
        {
            Debug.Log("No blueprints in enemy list");
            return;
        }

        int random = Random.Range(0, _bluePrints.Count);

        Blueprint blueprint = _bluePrints[random];
        Debug.Log($"Blueprint chosen: {blueprint.name}");
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
            //surrender logic here
            return false;
        }

        List<int> availableLanes = BoardLaneManager.Instance.GetAvailableEnemyLanes();

        foreach (BlueprintEntry turn in blueprint.Entries)
        {
            //check what type the entry is
            //if the entry type is random, call a different function to get a random card from that "tribe" or whatever its called
            EntryType type = turn.type;

            int randomIndex = Random.Range(0, availableLanes.Count);
            int lane = availableLanes[randomIndex];

            bool wasPlaced = false;

            availableLanes.RemoveAt(randomIndex); // move this after wasPlaced so we can remove the lane if the card was placed successfully

            if (type == EntryType.ExactCard)
            {
                CardModel model = new CardModel(turn.card);

                wasPlaced = await cardPlacer.HandlePlaceCard(_enemyDeck, model, lane);
                Debug.Log($"Type: {type}, Card Name: {turn.card.name}.");
            }
            else if (type == EntryType.RandomGroup)
            {

                 
                Debug.Log($"Type: {type}, Random group - random card from group will be taken");
            }

        }
        return true;
    }

    private void QueueNextCardInLane()
    {
        List<LaneSnapShot> lanes = CheckCurrentBoardState().Lanes;

        foreach (LaneSnapShot lane in lanes) // O(n) since only have max 4 lanes
        {
            //float score = EvaluateLane(lane);
            float score = eval.EvaluateLane(_aiProfile, lane);

            if (score >= 15)
            {
                Debug.Log($"<color=yellow> Lane {lane.LaneIndex + 1} has a score of {score} >= 15. Queuing strongest card available in this lane.</color>");

                CardModel currentStrongestCard = null;
                CardModel retrievedCard = cardRetriever.RetrieveCard(_aiProfile, lane);
                currentStrongestCard = retrievedCard;

                if (currentStrongestCard != null)
                {
                    _ = cardPlacer.HandlePlaceCard(_enemyDeck, currentStrongestCard, lane.LaneIndex);
                }
                continue;
            }

            else if (score > DAMAGE_THRESHOLD)
            {
                Debug.Log($"<color=yellow>Lane {lane.LaneIndex + 1} has a score of {score}. Queueing next card in this lane.</color>");

                foreach (CardModel card in _enemyDeck)
                {
                    if (card == null)
                    {
                        Debug.LogWarning($"Card that is being accessed is null");
                        continue;
                    }
                    if (lane.PlayerCard.HasValue && (card.Health > lane.PlayerCard.Value.Attack))
                    {
                        Debug.Log($"<color=cyan> This Card '{card.Name}' has enough health to survive the player's attack. Queueing this card in lane {lane.LaneIndex + 1}.</color>");

                        _ = cardPlacer.HandlePlaceCard(_enemyDeck, card, lane.LaneIndex);
                        break;
                    }
                    else if (!lane.PlayerCard.HasValue)
                    {
                        Debug.Log($"<color=cyan> There is no Player Card in this lane {lane.LaneIndex + 1}.</color>");
                        break;
                    }
                    else
                    {
                        Debug.Log($"<color=cyan> This card '{card.Name}' can't survive player attack");
                    }
                }
            }

            else
            {
                Debug.Log($"<color=white>Lane {lane.LaneIndex + 1} has a score of {score}. Not queueing next card in this lane.</color>");
            }
        }
    }
}
