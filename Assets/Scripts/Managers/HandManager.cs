using DG.Tweening;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;
using Scene = UnityEngine.SceneManagement.Scene;

public class HandManager : GenericSingleton<HandManager>
{
    public HandState CurrentHandState { get; private set; }

    [Header("Settings")]
    [SerializeField] private int maxHandSize;
    [SerializeField] private LayerMask _cardLayer;
    [SerializeField] private GameObject cardPrefab; // change this to specific card prefabs
    [SerializeField] private float cardOverlap = 0.15f;

    [Header("References")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform _viewToUsePoint;
    [SerializeField] private Transform _handPosition;
    [SerializeField] private CinemachineCamera _fpCamera;


    [Header("Listener to Event Channels")]
    [SerializeField] private CardEventChannelSO _cardClicked;

    [Header("Broadcast to Event Channels")]
    [SerializeField] private CHSEventChannelSO _cardUnselected;
    [SerializeField] private CHSEventChannelSO _cardPlayed;

    private bool _allowCardHover = true;
    private GameObject _currentHoveredCard;
    private Card _currentSelectedCard;
    private List<GameObject> _handCards = new();

    public Card CurrentSelectedCard => _currentSelectedCard;

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            DrawCard();
        }
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            ClearCards();
        }

        if (_allowCardHover)
        {
            HandleCardHover();
        }
    }

    private void OnEnable()
    {
        _cardClicked.onEventRaised += CardTempLeave;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        _cardClicked.onEventRaised -= CardTempLeave;
        InputManager.Instance.OnBackButtonPressed -= CardBackToHand;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindSceneDependencies();
    }

    private void FindSceneDependencies() // Move this
    {
        _viewToUsePoint = GameObject.FindWithTag("ViewToUse").transform;
        _fpCamera = GameObject.FindWithTag("FPCamera").transform.GetComponent<CinemachineCamera>();
        CurrentHandState = HandState.InHand; // change
        InputManager.Instance.OnBackButtonPressed += CardBackToHand;
    }

    private void DrawCard()
    {
        if(_handCards.Count >= maxHandSize) return;
        GameObject newCard = Instantiate(cardPrefab, spawnPoint.position, spawnPoint.rotation);
        _handCards.Add(newCard);
        UpdateCardPosition();
    }

    private void UpdateCardPosition()
    {
        Vector3 handCenter = _handPosition.position;

        for (int i = 0; i < _handCards.Count; i++)
        {
            float offset = i - (_handCards.Count - 1) / 2f;
            Vector3 position = handCenter + _fpCamera.transform.right * (offset * cardOverlap);

            // last card (highest index) is closest to camera
            position -= _fpCamera.transform.forward * (i * 0.01f);

            Quaternion rotation = Quaternion.Euler(-90f, 180f, 0f);

            Card card = _handCards[i].GetComponent<Card>();
            card._basePosition = position;
            card._baseRotation = rotation;

            _handCards[i].transform.DOKill();
            _handCards[i].transform.DOMove(position, 0.25f);
            _handCards[i].transform.DORotateQuaternion(rotation, 0.25f);
        }
    }

    private void HandleCardHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.MouseScreenPosition);
        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _cardLayer))
        {
            GameObject hitCard = hit.collider.transform.root.gameObject; // handCards contains the root gameObject of the card prefab, so we need to get the root of the hit collider

            if (hitCard == _currentHoveredCard)
            {
                return; // Already hovering this card, do nothing
            }
            if(_currentHoveredCard != null)
            {
                _currentHoveredCard.GetComponent<IHoverable>()?.OnHoverExit();
            }
            //Enter new, only if it's a card in hand
            if(_handCards.Contains(hitCard))
            {
                _currentHoveredCard = hitCard;
                _currentHoveredCard.GetComponent<IHoverable>()?.OnHoverEnter();
            }
            else
            {
                _currentHoveredCard = null;
            }
        }
        else
        {
            //ray cast hit nothing
            if(_currentHoveredCard != null)
            {
                _currentHoveredCard.GetComponent<IHoverable>()?.OnHoverExit();
                _currentHoveredCard = null;
            }
        }
    }

    public void ClearCards()
    {
        if (_handCards == null || _handCards.Count == 0) return;
        foreach(var card in _handCards)
        {
            Destroy(card);
        }
        _handCards.Clear();
    }

    private void CardTempLeave(Card card)
    {
        Debug.Log("Num of cards in hand before playing: " + _handCards.Count);
        _allowCardHover = false;
        _currentSelectedCard = card;
        _handCards.Remove(card.gameObject);

        SwitchHandState(HandState.Selected);
        _currentSelectedCard.transform.DOMove(_viewToUsePoint.position, 0.3f);
        _currentSelectedCard.transform.DORotateQuaternion(Quaternion.Euler(180f, -180f, 0), 0.3f);
    }

    private void CardBackToHand()
    {
        if (_currentSelectedCard == null) return;

        _handCards.Add(_currentSelectedCard.gameObject);

        _currentSelectedCard.transform.DOKill();

        Quaternion rotation = _currentSelectedCard.GetComponent<Card>()._baseRotation;
        _currentSelectedCard.transform.DORotateQuaternion(rotation, 0.25f);

        _currentSelectedCard = null;
        _allowCardHover = true;

        SwitchHandState(HandState.InHand);
        _cardUnselected.RaiseEvent(CurrentHandState);
        UpdateCardPosition();
    }

    public void PlayCurrentCard(CardDropArea targetArea)
    {
        if (_currentSelectedCard == null || targetArea.SlotFaction != Faction.Player) return;
        Debug.Log("Num of cards in hand after playing: " + _handCards.Count);

        Card playedCard = _currentSelectedCard;
        _currentSelectedCard = null;

        playedCard.transform.DOKill();
        playedCard.transform.DOMove(targetArea.transform.position, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            Debug.Log("OnComplete fired");
            playedCard.CardIsPlayed();
            targetArea.OnCardDrop();
            _allowCardHover = true;
        });

        SwitchHandState(HandState.InHand);
        _cardPlayed.RaiseEvent(CurrentHandState);
        UpdateCardPosition();
    }

    private void OnHandFocus()
    {
        // this should be called when we press an input, like a button or something
    }

    public void SwitchHandState(HandState state)
    {
        
        if(CurrentHandState != state)
        {
            CurrentHandState = state;
        }
    }
}
