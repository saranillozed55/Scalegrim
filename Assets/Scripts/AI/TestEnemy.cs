using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.Multiplayer.PlayMode;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public enum CardPreference
{
    OVERPOWER,
    TANK,
}

public class TestEnemy : MonoBehaviour
{
    [SerializeField] private int _maxCards = 5; // enemy shouldnt have _maxCards unless we want it to
    [SerializeField] private Card _cardPrefab;

    [SerializeField] private List<EnemyPrepArea> _prepArea;

    [SerializeField] private List<Card> _enemyDeck = new();

    [Header("Listener to Event Channels")]
    [SerializeField] private VoidEventChannel _onEnemyEndTurn;

    [Header("Settings for Next Card Moves")]
    [SerializeField] private float DAMAGE_THRESHOLD = 5f;

    [Header("AI profile")]
    [SerializeField] private AIPersonality _aiProfile;

    BoardEvaluater eval = new BoardEvaluater();
    CardPlacer cardPlacer = new CardPlacer();
    CardRetriever cardRetriever;

    private void OnEnable()
    {
        _onEnemyEndTurn.onEventRaised += QueueNextCardInLane;
    }

    private void OnDisable()
    {
        _onEnemyEndTurn.onEventRaised -= QueueNextCardInLane;
    }

    private void Start()
    {
        BoardLaneManager.Instance.PlaceEnemyCardsInQueue(_cardPrefab, 0, out bool full1);
        BoardLaneManager.Instance.PlaceEnemyCardsInQueue(_cardPrefab, 2, out bool full2);

        cardRetriever = new CardRetriever(_enemyDeck);
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

                Card currentStrongestCard = null;
                Card retrievedCard = cardRetriever.RetrieveCard(_aiProfile, lane);
                currentStrongestCard = retrievedCard;

                if (currentStrongestCard != null)
                {
                    //HandlePlaceCard(currentStrongestCard, lane.LaneIndex);
                    cardPlacer.HandlePlaceCard(_enemyDeck, currentStrongestCard, lane.LaneIndex);
                }
                continue;
            }

            else if (score > DAMAGE_THRESHOLD)
            {
                Debug.Log($"<color=yellow>Lane {lane.LaneIndex + 1} has a score of {score}. Queueing next card in this lane.</color>");

                foreach (Card card in _enemyDeck)
                {
                    if (card == null)
                    {
                        Debug.LogWarning($"Card that is being accessed is null");
                        continue;
                    }
                    if (lane.PlayerCard.HasValue && (card.BaseHealth > lane.PlayerCard.Value.Attack))
                    {
                        Debug.Log($"<color=cyan> This Card '{card.name}' has enough health to survive the player's attack. Queueing this card in lane {lane.LaneIndex + 1}.</color>");

                        //HandlePlaceCard(card, lane.LaneIndex);
                        cardPlacer.HandlePlaceCard(_enemyDeck, card, lane.LaneIndex);
                        break;
                    }
                    else if (!lane.PlayerCard.HasValue)
                    {
                        Debug.Log($"<color=cyan> There is no Player Card in this lane {lane.LaneIndex + 1}.</color>");
                        break;
                    }
                    else
                    {
                        Debug.Log($"<color=cyan> This card '{card.name}' can't survive player attack");
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
