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

    private bool _allowCardHover = true;
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

        if (_allowCardHover)
        {
            HandleCardHover();
        }
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

    public bool DrawCard(CardView newCard)
    {
        if (newCard == null) return false;
        if (_handCards.Count >= maxHandSize) return false;
        _handCards.Add(newCard);
        newCard.CardModel.SetHoverable(false);
        
        UpdateCardPosition();
        return true;
    }

    private async void UpdateCardPosition()
    {
        Vector3 handCenter = _handPosition.position;

        for (int i = 0; i < _handCards.Count; i++)
        {
            float offset = i - (_handCards.Count - 1) / 2f;
            Vector3 position = handCenter + _fpCamera.transform.right * (offset * cardOverlap);

            // last card (highest index) is closest to camera
            position -= _fpCamera.transform.forward * (i * 0.01f);

            //MOVE THIS TO CARDVIEW I BELIEVE
            CardView cardView = _handCards[i];
            cardView.SetBasePosition(position);
            cardView.SetBaseRotation(_handPosition.rotation);

            try
            {
                await cardView.MoveCardToPosition(position, _handPosition.rotation);
                cardView.CardModel.SetHoverable(true);
            }
            catch(Exception e)
            {
                Debug.LogError("Error[Hand Manager]: " + e);
            }
        }
    }

    private void HandleCardHover()
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

    public async void CardTempLeave(CardView card)
    {
        _allowCardHover = false;
        _handCards.Remove(card);

        SwitchHandState(HandState.Selected);

        try
        {
           await card.MoveCardToPosition(_viewToUsePoint.position, CardRotations._cardFaceFlatUp);
        }

        catch(Exception e)
        {
            Debug.LogError("Error[Hand Manager]: " + e.Message);
        }
    }

    public async Task CardBackToHand(CardView card)
    {
        _handCards.Add(card);

        await card.RotateCard(card.BaseRotation);

        _allowCardHover = true;

        SwitchHandState(HandState.InHand);
        UpdateCardPosition();
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
        _allowCardHover = true;

        //playedCard.transform.DOKill();
        //playedCard.transform.DOMove(targetArea.transform.position, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
        //{
        //    playedCard.CardModel.PlayCard();
        //    targetArea.OnCardDrop(playedCard);
        //    _allowCardHover = true;
        //});

        SwitchHandState(HandState.InHand);
        UpdateCardPosition();
    }

    public void SwitchHandState(HandState state)
    {
        if (CurrentHandState != state)
        {
            CurrentHandState = state;
        }
    }
}
