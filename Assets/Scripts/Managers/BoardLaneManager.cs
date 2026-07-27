using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Analytics;
using System;

public class BoardLaneManager : GenericSingleton<BoardLaneManager>
{
    [SerializeField] private List<EnemyPrepArea> _prepAreas;
    [SerializeField] private List<LaneView> physicalLanes;
    private List<Lane> logicLanes = new List<Lane>();

    [Header("Broadcast to EventChannels")]
    [SerializeField] private VoidEventChannel _onCombatStart;

    [Header("Listener to EventChannels")]
    [SerializeField] private VoidEventChannel _onPlayerEndTurn;

    public List<Lane> LogicLanes => logicLanes;

    protected override void Awake()
    {
        base.Awake();
        InitializeBoard();
    }

    private void OnEnable()
    {
        _onPlayerEndTurn.onEventRaised += AdvanceEnemyCardsFromQueue;
    }
    private void OnDisable()
    {
        _onPlayerEndTurn.onEventRaised -= AdvanceEnemyCardsFromQueue;
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

    public void PlaceCardInLane(CardModel card, int laneIndex, Owner slotOwner)
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
        Debug.Log($"<color=#4FC3F7>[Card]</color> {card.Name} → Lane {laneIndex}, Owner {slotOwner}");
        logicLanes[laneIndex] = updatedLane;
    }

    public void PlaceEnemyCardsInQueue(CardView cardViewPrefab, int laneIndex, out bool full)
    {
        EnemyPrepArea targetPrepArea = _prepAreas[laneIndex];
        if (targetPrepArea != null && !targetPrepArea.HasCard && targetPrepArea.FrontCardDropArea.IsFull())
        {
            full = true;
            return;
        }

        full = false;
        GameObject instance = Instantiate(cardViewPrefab.gameObject, targetPrepArea._cardSpawnLocation.position, targetPrepArea._cardSpawnLocation.rotation);

        CardView cardInstance = instance.GetComponent<CardView>();
        cardInstance.CardModel.PlayCard();

        logicLanes[laneIndex].EnemyQueuedCard = cardInstance.CardModel;
        Vector3 targetPosition = targetPrepArea.transform.position;

        instance.transform.DOKill();
        instance.transform.DOMove(targetPosition, 0.3f);
        instance.transform.DORotateQuaternion(CardRotations._cardFaceFlatUp, 0.3f).OnComplete(() => targetPrepArea.OnCardDrop(cardInstance));
    }

    public async void AdvanceEnemyCardsFromQueue()
    {
        //list of all awaitable animations so we can wait for them
        List<Awaitable> animationTasks = new List<Awaitable>();
        foreach (LaneView lane in physicalLanes)
        {
            if (lane.EnemyPrepArea.HasCard && lane.EnemyActiveArea.IsFull())
            {
                Debug.Log("There is already a card infront of this lane, cannot advance queued card");
                continue;
            }
            if (!lane.EnemyPrepArea.HasCard) continue;

            //pop card out of the prep area directly
            CardView card = lane.EnemyPrepArea.TriggerPush();

            if (card != null)
            {
                Lane matchingDataLane = logicLanes.FirstOrDefault(l => l.LaneIndex == lane.laneIndex);
                if (matchingDataLane != null)
                {
                    //start async animation and add to list
                    animationTasks.Add(HandleCardAdvanceAsync(matchingDataLane, lane, card));
                }
            }
        }

        if (animationTasks.Count == 0)
        {
            _onCombatStart.RaiseEvent();
            return;
        }

        //wait for all animations to finish in the list
        foreach (Awaitable anim in animationTasks)
        {
            await anim;
        }

        _onCombatStart.RaiseEvent();
    }

    private async Awaitable HandleCardAdvanceAsync(Lane dataLane, LaneView view, CardView card)
    {
        try
        {
            dataLane.EnemyActiveCard = card.CardModel;
            dataLane.EnemyQueuedCard = null;

            card.transform.DOKill();
            Transform targetLocation = view.EnemyActiveArea.transform;
            Quaternion targetRotation = CardRotations._cardFaceFlatUp; // WANT TO CHANGE THIS LATER TO MIMIC INSCRYPTION MAYBE

            Sequence animSequence = DOTween.Sequence();

            animSequence.Join(card.transform.DOMove(targetLocation.position, 0.3f).SetEase(Ease.OutQuad));
            animSequence.Join(card.transform.DORotateQuaternion(CardRotations._cardFaceFlatUp, 0.3f));

            await animSequence.AsyncWaitForCompletion();

            view.EnemyActiveArea.OnCardDrop(card);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error in HandleCardAdvanceAsync: {ex.Message}");
        }
    }

    public BoardState CaptureBoardState()
    {
        BoardState state = new BoardState()
        {
            Lanes = new List<LaneSnapShot>()
        };

        foreach(var lane in logicLanes)
        {
            LaneSnapShot snapShot = new LaneSnapShot
            {
                LaneIndex = lane.LaneIndex,
                EnemyCard = ToSnapShot(lane.EnemyActiveCard),
                PlayerCard = ToSnapShot(lane.PlayerActiveCard),
                EnemyQueuedCard = ToSnapShot(lane.EnemyQueuedCard)
            };
            state.Lanes.Add(snapShot);
        }

        return state;
    }

    public CardSnapShot? ToSnapShot(CardModel card)
    {
        if (card == null) return null;

        return new CardSnapShot
        {
            CardName = card.Name,
            Attack = card.AttackDamage,
            Health = card.Health,
        };
    }
}
