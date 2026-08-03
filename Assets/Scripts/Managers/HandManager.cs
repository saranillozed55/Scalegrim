using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    [SerializeField] private CardView cardViewPrefab; // change this to specific card prefabs
    [SerializeField] private float cardOverlap = 0.15f;

    [Header("References")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform _viewToUsePoint;
    [SerializeField] private Transform _handPosition;
    [SerializeField] private CinemachineCamera _fpCamera;

    private MousePosition _mousePosition;

    private CardView _currentHoveredCard;
    private List<CardView> _handCards = new();

    protected override void Awake()
    {
        base.Awake();
        _mousePosition = FindFirstObjectByType<MousePosition>();
    }

    private void Update()
    {
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            ClearCards();
        }

        HandleCardHover();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnBackButtonPressed -= SelectionManager.Instance.DeselectCard;
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
        InputManager.Instance.OnBackButtonPressed += SelectionManager.Instance.DeselectCard;
    }
    public async Task<bool> DrawCard(CardView newCard)
    {
        if (newCard == null) return false;
        if (_handCards.Count >= maxHandSize) return false;
        _handCards.Add(newCard);
        newCard.CardModel.SetHoverable(false);

        await UpdateCardPosition();
        return true;
    }

    private async Task UpdateCardPosition()
    {
        Vector3 handCenter = _handPosition.position;

        List<Task> tasks = new();

        for (int i = 0; i < _handCards.Count; i++)
        {
            float offset = i - (_handCards.Count - 1) / 2f;
            Vector3 position = handCenter + _fpCamera.transform.right * (offset * cardOverlap);

            // last card (highest index) is closest to camera
            position -= _fpCamera.transform.forward * (i * 0.01f);

            CardView cardView = _handCards[i];
            cardView.SetBasePosition(position);
            cardView.SetBaseRotation(_handPosition.rotation);

            tasks.Add(cardView.MoveCardToPosition(position, _handPosition.rotation));
            //cardView.CardModel.SetHoverable(true);
        }

        await Task.WhenAll(tasks);

        foreach(var card in _handCards)
        {
            card.CardModel.SetHoverable(true);
        }
    }

    private void HandleCardHover() // maybe move this to not in manager and let card view handle its own hover state, but manager can handle the raycast and tell the card to hover
    {
        if (Physics.Raycast(_mousePosition.GetMouseRay(), out RaycastHit hit, Mathf.Infinity, _cardLayer))
        {
            // handCards contains the root CardView of the card prefab, so get the root's component
            CardView hitCard = hit.collider.transform.root.GetComponent<CardView>();

            if (hitCard == _currentHoveredCard)
            {
                return; // Already hovering this card, do nothing
            }
            if (_currentHoveredCard != null)
            {
                _currentHoveredCard.OnHoverExit();
            }
            // Enter new, only if it's a card in hand
            if (hitCard != null && _handCards.Contains(hitCard))
            {
                _currentHoveredCard = hitCard;
                _currentHoveredCard.OnHoverEnter();
            }
            else
            {
                _currentHoveredCard = null;
            }
        }
        else
        {
            // raycast hit nothing
            if (_currentHoveredCard != null)
            {
                _currentHoveredCard.OnHoverExit();
                _currentHoveredCard = null;
            }
        }
    }

    public void ClearCards()
    {
        if (_handCards == null || _handCards.Count == 0) return;
        foreach (var card in _handCards)
        {
            Destroy(card.gameObject);
        }
        _handCards.Clear();
    }

    public void CardTempLeave(CardView card)
    {
        _handCards.Remove(card);

        SwitchHandState(HandState.Selected);

        _ = card.MoveCardToPosition(_viewToUsePoint.position, CardRotations._cardFaceFlatUp);
    }

    public async Task CardBackToHand(CardView card)
    {
        _handCards.Add(card);

        SwitchHandState(HandState.InHand);

        await UpdateCardPosition();
    }

    public async Task PlayCurrentCard(CardDropArea targetArea)
    {
        if (targetArea.SlotOwner != Owner.Player) return;

        CardView playedCard = SelectionManager.Instance.SelectedHandCard;
        SelectionManager.Instance.CardPlayedDeselect();
        if (playedCard == null) return;

        await playedCard.MoveCardToPosition(targetArea.transform.position);
        playedCard.CardModel.PlayCard();
        targetArea.OnCardDrop(playedCard);

        SwitchHandState(HandState.InHand);
        await UpdateCardPosition();
    }

    public void SwitchHandState(HandState state)
    {
        if (CurrentHandState != state)
        {
            CurrentHandState = state;
        }
    }
}
