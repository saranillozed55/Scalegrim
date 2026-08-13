using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Analytics;
using System;
using System.Threading.Tasks;

public class BoardLaneManager : GenericSingleton<BoardLaneManager>, ICardSpawner
{
    public Transform spawnerTransform { get; set; }
    [SerializeField] private Transform refSpawnerTransform;
    [SerializeField] private List<EnemyPrepArea> _prepAreas;
    [SerializeField] private List<LaneView> physicalLanes;

    [Header("Broadcast to EventChannels")]
    [SerializeField] private VoidEventChannel _onCombatStart;

    [Header("Listener to EventChannels")]
    [SerializeField] private VoidEventChannel _onPlayerEndTurn;

    private List<Lane> logicLanes = new List<Lane>();

    public IReadOnlyList<Lane> LogicLanes => logicLanes;

    private LaneBuilder laneBuilder = new LaneBuilder(); // not sure when to use this yet
    private CardSpawner spawner;

    protected override void Awake()
    {
        base.Awake();
        spawnerTransform = refSpawnerTransform;
    }

    private void Start()
    {
        spawner = new CardSpawner();
    }

    private void OnEnable()
    {
        _onPlayerEndTurn.onEventRaised += AdvanceEnemyCardsFromQueue;
    }

    private void OnDisable()
    {
        _onPlayerEndTurn.onEventRaised -= AdvanceEnemyCardsFromQueue;
    }

    public void InitializeBoard(EncounterData encounterData)
    {
        logicLanes.Clear();

        foreach (LaneView view in physicalLanes)
        {
            Lane laneData = encounterData.GetLane(view.laneIndex);
            view.Init(laneData);
            logicLanes.Add(laneData);
        }
    }

    private void LoadBoard()
    {
        
    }

    public void PlaceCardInLane(CardModel card, int laneIndex, AreaOwnerType slotOwner)
    {
        Lane updatedLane = logicLanes[laneIndex];
        if (slotOwner == AreaOwnerType.EnemyActive)
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

    public async Task<bool> PlaceEnemyCardsInQueue(CardModel model, int laneIndex)
    {
        if (model == null)
        {
            Debug.LogWarning("PlaceEnemyCardsInQueue: model is null");
            return false;
        }
        if(model.ViewPrefab == null)
        {
            Debug.LogWarning($"PlaceEnemyCardsInQueue: Card ViewPrefab is null for card{model.Name}");
            return false;
        }
        if (laneIndex < 0 || laneIndex >= _prepAreas?.Count)
        {
            Debug.LogWarning($"PlaceEnemyCardsInQueue: laneIndex {laneIndex} is out of range");
            return false;
        }
        EnemyPrepArea targetPrepArea = _prepAreas[laneIndex];

        if (!targetPrepArea.HasCard && targetPrepArea.FrontCardDropArea.IsAreaTaken())
        {
            return false;
        }

        //CardView instance = Instantiate(model.ViewPrefab, targetPrepArea._cardSpawnLocation.position, targetPrepArea._cardSpawnLocation.rotation, spawnerTransform);
        CardView instance = spawner.SpawnDesignatedCard(model.ViewPrefab, targetPrepArea._cardSpawnLocation, spawnerTransform);
        instance.InitCard(model);
        instance.CardModel.PlayCard();

        logicLanes[laneIndex].EnemyQueuedCard = instance.CardModel;
        Vector3 targetPosition = targetPrepArea.transform.position;
        await instance.MoveCardToPosition(targetPosition, CardRotations._cardFaceFlatUp);
        targetPrepArea.OnCardDrop(instance);

        return true;
    }

    public async void AdvanceEnemyCardsFromQueue()
    {
        //list of all awaitable animations so we can wait for them
        List<Awaitable> animationTasks = new List<Awaitable>();
        foreach (LaneView lane in physicalLanes)
        {
            if (lane.EnemyPrepArea.HasCard && lane.EnemyActiveArea.IsAreaTaken())
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

            Transform targetLocation = view.EnemyActiveArea.transform;
            Quaternion targetRotation = CardRotations._cardFaceFlatUp; // WANT TO CHANGE THIS LATER TO MIMIC INSCRYPTION MAYBE

            await card.MoveCardToPosition(targetLocation.position);

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
        if (card == null || card.Dead) return null;

        return new CardSnapShot
        {
            cardModel = card,
            //CardName = card.Name,
            //Attack = card.AttackDamage,
            //Health = card.Health,
        };
    }

    public List<int> GetAvailableEnemyLanes()
    {
        List<int> availableLanes = new();

        for(int i = 0; i < logicLanes.Count; i++)
        {
            if (!logicLanes[i].IsEnemySideOccupied)
            {
                availableLanes.Add(i);
            }
        }
        return availableLanes;
    }

    public void RemoveCardFromLane(CardModel deadCard)
    {
        for(int i = 0; i < logicLanes.Count; i++ )
        {
            Lane lane = logicLanes[i];

            if (lane.EnemyActiveCard == deadCard)
            {
                lane.EnemyActiveCard = null;
            }
            else if(lane.PlayerActiveCard == deadCard)
            {
                lane.PlayerActiveCard = null;
            }
            else if(lane.EnemyQueuedCard == deadCard)
            {
                lane.EnemyQueuedCard = null;
            }
            else
            {
                continue;
            }
            logicLanes[i] = lane;
            return;
        }
    }
}
