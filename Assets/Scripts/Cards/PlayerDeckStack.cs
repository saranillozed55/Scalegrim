using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

//in obsidian, said that maybe not use ICardSpawner and instead use a script to spawn in cards rather than multiple scripts trying to spawn in cards
public class PlayerDeckStack : MonoBehaviour, IClickable, ICardSpawner
{
    public Transform spawnerTransform { get; set; }
    [SerializeField] private Transform refSpawnerTransform;
    private bool _stackLoaded = false;
    private Transform _playerDeckStackPosition;
    [SerializeField] private float _gapSize = 0.02f;
    [SerializeField] private Transform _spawnLocation;
    [SerializeField] private PlayerDeck _playerDeck;

    private Stack<CardView> _deckCards = new();
    private CardSpawner spawner; 

    private bool canDrawCards = false;
    private bool _isPopping = false;
    private bool waitDrawCard = false; // use for waiting until the player has drawn the card, because want the camera to be facing deck so player can draw first before playing cards

    [Header("Listener to Event Channels")]
    [SerializeField] private VoidEventChannel _onPlayerStartTurn;

    private void Awake()
    {
        spawnerTransform = refSpawnerTransform;
    }

    private void OnEnable()
    {
        _onPlayerStartTurn.onEventRaised += SetMustWaitForDrawCard;
    }

    private void OnDisable()
    {
        _onPlayerStartTurn.onEventRaised -= SetMustWaitForDrawCard;
    }

    private void Start()
    {
        spawner = new CardSpawner();
        _playerDeckStackPosition = GetComponent<Transform>();
        canDrawCards = true;

        _ = InitializeDeckAsync();
    }

    private async Task InitializeDeckAsync()
    {
        bool deckLoaded = await LoadDeck(_playerDeck.Deck);

        if(deckLoaded)
        {
            for (int i = 0; i < 3; i++)
            {
                CardView poppedCardView = _deckCards.Pop();
                HandManager.Instance.DrawCard(poppedCardView);
                await Task.Delay(400);
            }
        }
    }

    private void Update()
    {
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            ClearDeckStack();
        }
    }

    public async Task<bool> LoadDeck(List<CardModel> deckCards)
    {
        deckCards.Shuffle();
        List<Task> taskList = new List<Task>();

        for (int i = 0; i < deckCards.Count; i++)
        {
            //CardView instance = Instantiate(deckCards[i].ViewPrefab, _spawnLocation.position, _spawnLocation.rotation, spawnerTransform);
            CardView instance = spawner.SpawnDesignatedCard(deckCards[i].ViewPrefab, _spawnLocation.transform, spawnerTransform);
            instance.InitCard(deckCards[i]);

            float delay = i * 0.08f;
            Vector3 position = _playerDeckStackPosition.position + (Vector3.up * _gapSize * i);

            taskList.Add(instance.MoveCardToPositionWithDelay(position, CardRotations._cardFaceFlatDown, delay));
            _deckCards.Push(instance);
        }

        await Task.WhenAll(taskList);

        return _stackLoaded = true;
    }

    public void OnClick() //event handler is fine for async void in this case since we don't need to await it
    {
        if (CantDrawCards()) return;

        _isPopping = true;

        CardView poppedCard = _deckCards.Pop();
        bool drawn = HandManager.Instance.DrawCard(poppedCard);

        if (drawn && waitDrawCard)
        {
            CinemachineSwitcher.Instance.SwitchState(CameraState.FPCamera);
            waitDrawCard = false;
            canDrawCards = false;
        }

        if (!drawn) // not sure if this ever runs
        {
            _deckCards.Push(poppedCard);
        }

        _isPopping = false;
    }


    private bool CantDrawCards()
    {
        return (_isPopping || !canDrawCards || !_stackLoaded || _deckCards.Count == 0);
    }

    public void SetMustWaitForDrawCard()
    {
        waitDrawCard = true;
        canDrawCards = true;
    }

    public void ClearDeckStack()
    {
        if (_deckCards.Count > 0)
        {
            foreach (CardView card in _deckCards)
            {
                Destroy(card.gameObject);
            }
            _deckCards.Clear();
            _stackLoaded = false;
        }
    }
}
