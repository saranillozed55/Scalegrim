using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using UnityEngine.Analytics;

public class BoardLaneManager : GenericSingleton<BoardLaneManager>
{
    [SerializeField] private int _totalLanes = 3;
    [SerializeField] private List<EnemyPrepArea> _prepAreas;
    [SerializeField] private List<LaneView> physicalLanes;

    public List<Lane> logicLanes = new List<Lane>();

    [Header("Broadcast to EventChannels")]
    [SerializeField] private VoidEventChannel _onCombatStart;

    [Header("Listener to EventChannels")]
    [SerializeField] private VoidEventChannel _onPlayerEndTurn;

    protected override void Awake()
    {
        base.Awake();
        InitializeBoard();
    }

    private void OnEnable()
    {
        _onPlayerEndTurn.onEventRaised += AdvanceEnemyCardsFromQueue;
        foreach (LaneView view in physicalLanes)
        {
            view.EnemyPrepArea.pushedCard += OnPrepAreaPushedCard;
        }
    }
    private void OnDisable()
    {
        _onPlayerEndTurn.onEventRaised -= AdvanceEnemyCardsFromQueue;
        foreach (LaneView view in physicalLanes)
        {
            view.EnemyPrepArea.pushedCard -= OnPrepAreaPushedCard;
        }
    }

    private void InitializeBoard()
    {
        logicLanes.Clear();
        foreach (LaneView view in physicalLanes)
        {

            Lane dataLane = new Lane { LaneIndex = view.laneIndex };
            logicLanes.Add(dataLane);
        }
    }
    private void OnPrepAreaPushedCard(Card card, LaneView view, System.Action onComplete)
    {
        Lane matchingDataLane = null;
        foreach (Lane dataLane in logicLanes)
        {
            if (dataLane.LaneIndex == view.laneIndex)
            {
                matchingDataLane = dataLane;
                break;
            }
        }
        if (matchingDataLane == null || view == null) return;
        HandleCardAdvance(matchingDataLane, view, card, onComplete);
    }

    public void PlaceCardInLane(Card card, int laneIndex, Owner slotOwner)
    {
        Lane updatedLane = logicLanes[laneIndex];
        if (slotOwner == Owner.Enemy)
        {
            updatedLane.EnemyActiveCard = card;
        }
        else
        {
            updatedLane.PlayerActiveCard = card;
        }
        Debug.Log("Card: " + card + " Lane index: " + laneIndex + "Slot Owner: " + slotOwner);
        logicLanes[laneIndex] = updatedLane;
    }

    public void PlaceEnemyCardsInQueue(Card cardPrefab, int laneIndex)
    {
        EnemyPrepArea targetPrepArea = _prepAreas[laneIndex];

        GameObject instance = Instantiate(cardPrefab.gameObject, targetPrepArea._cardSpawnLocation.position, targetPrepArea._cardSpawnLocation.rotation);

        Card cardInstance = instance.GetComponent<Card>();
        logicLanes[laneIndex].EnemyQueuedCard = cardInstance;
        Vector3 targetPosition = targetPrepArea.transform.position;

        instance.transform.DOKill();
        instance.transform.DOMove(targetPosition, 0.3f);
        instance.transform.DORotateQuaternion(CardRotations._cardFaceFlatUp, 0.3f).OnComplete(() => targetPrepArea.OnCardDrop(cardInstance));
    }

    public void AdvanceEnemyCardsFromQueue()
    {
        int movingCardsCount = 0;
        foreach (LaneView lane in physicalLanes)
        {
            if (lane.EnemyPrepArea.HasCard)
            {
                movingCardsCount++;
            }
        }
        if (movingCardsCount == 0)
        {
            _onCombatStart.RaiseEvent();
            return;
        }
        foreach (LaneView lane in physicalLanes)
        {
            if (!lane.EnemyPrepArea.HasCard) continue;

            lane.EnemyPrepArea.TriggerPush(() =>
            {
                movingCardsCount--;
                if (movingCardsCount == 0)
                {
                    _onCombatStart.RaiseEvent();
                }
            });
        }
    }

    private void HandleCardAdvance(Lane dataLane, LaneView view, Card card, System.Action onCompleteCallback)
    {
        dataLane.EnemyActiveCard = card;
        dataLane.EnemyQueuedCard = null;

        Transform targetTransform = view.EnemyActiveArea.transform;
        Quaternion targetRotation = CardRotations._cardFaceFlatUp;

        card.transform.DOKill();
        card.transform.DOMove(targetTransform.position, 0.3f);
        card.transform.DORotateQuaternion(targetRotation, 0.3f).OnComplete(() =>
        {
            view.EnemyActiveArea.OnCardDrop(card);

            onCompleteCallback?.Invoke();
        });
    }
}
