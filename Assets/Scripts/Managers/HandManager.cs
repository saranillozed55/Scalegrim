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

    private MousePosition _mousePosition;
    
    private bool _allowCardHover = true;
    private GameObject _currentHoveredCard;
    private List<GameObject> _handCards = new();

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

    public bool DrawCard(GameObject newCard)
    {
        if(_handCards.Count >= maxHandSize) return false;
        _handCards.Add(newCard);
        newCard.GetComponent<Card>().SetHoverable(false);
        UpdateCardPosition();
        return true;
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

            Card card = _handCards[i].GetComponent<Card>();
            card._basePosition = position;
            card._baseRotation = _handPosition.rotation;

            _handCards[i].transform.DOKill();
            _handCards[i].transform.DOMove(position, 0.25f).OnComplete(() => card.SetHoverable(true));
            _handCards[i].transform.DORotateQuaternion(_handPosition.rotation, 0.25f);

            
        }
    }

    private void HandleCardHover()
    {
        if(Physics.Raycast(_mousePosition.GetMouseRay(), out RaycastHit hit, Mathf.Infinity, _cardLayer))
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

    public void CardTempLeave(Card card)
    {
        _allowCardHover = false;
        _handCards.Remove(card.gameObject);

        SwitchHandState(HandState.Selected);
        card.transform.DOKill();
        card.transform.DOMove(_viewToUsePoint.position, 0.3f);
        card.transform.DORotateQuaternion(CardRotations._cardFaceFlatUp, 0.3f);
    }

    public void CardBackToHand(Card card)
    {
        _handCards.Add(card.gameObject);

        card.transform.DOKill();

        card.transform.DORotateQuaternion(card._baseRotation, 0.25f);

        _allowCardHover = true;

        SwitchHandState(HandState.InHand);

        UpdateCardPosition();
    }

    public void PlayCurrentCard(CardDropArea targetArea)
    {
        if (targetArea.SlotOwner != Owner.Player) return;

        Card playedCard = SelectionManager.Instance.SelectedHandCard;
        SelectionManager.Instance.CardPlayedDeselect();
        if (playedCard == null) return;

        playedCard.transform.DOKill();
        playedCard.transform.DOMove(targetArea.transform.position, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            playedCard.CardIsPlayed();
            targetArea.OnCardDrop(playedCard); 
            _allowCardHover = true;
        });

        SwitchHandState(HandState.InHand);
        UpdateCardPosition();
    }

    public void SwitchHandState(HandState state)
    {
        if(CurrentHandState != state)
        {
            CurrentHandState = state;
        }
    }
}
