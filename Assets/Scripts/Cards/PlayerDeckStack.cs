using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDeckStack : MonoBehaviour, IClickable
{
    private Transform _playerDeckStackPosition;
    private bool _stackLoaded = false;

    [SerializeField] private float _gapSize = 0.02f;
    [SerializeField] private Transform _spawnLocation;
    [SerializeField] private PlayerDeck _playerDeck;
    [SerializeField] private CardView _cardViewPrefab;

    private Stack<CardView> _deckCards = new();

    private bool _isPopping = false;

    private void Start()
    {
        _playerDeckStackPosition = GetComponent<Transform>();
    }

    private void Update()
    {
        if(Keyboard.current.lKey.wasPressedThisFrame && !_stackLoaded)
        {
            LoadDeck(_playerDeck.Deck);
        }
        if(Keyboard.current.kKey.wasPressedThisFrame)
        {
            ClearDeckStack();
        }
    }
    public void LoadDeck(List<CardModel> deckCards)
    {
        for (int i = 0; i < deckCards.Count; i++)
        {
            CardView instance = Instantiate(_cardViewPrefab, _spawnLocation.position, _spawnLocation.rotation);
            instance.InitCard(deckCards[i]);

            float delay = i * 0.08f;
            Vector3 position = _playerDeckStackPosition.position + (Vector3.up * _gapSize * i);

            instance.transform.DOKill();
            instance.transform.DOMove(position, 0.3f).SetDelay(delay);

            instance.transform.DORotateQuaternion(CardRotations._cardFaceFlatDown, 0.3f).SetDelay(delay); 
            _deckCards.Push(instance);
        }
        _stackLoaded = true;
    }

    public void OnClick()
    {
        if (_isPopping) return;
        if (_deckCards.Count == 0) return;

        _isPopping = true;

        CardView poppedCard = _deckCards.Pop();
        bool drawn = HandManager.Instance.DrawCard(poppedCard);

        if(!drawn)
        {
            _deckCards.Push(poppedCard);
        }

        //small delay matching draw tween so that we don't double click and it pops twice
        DOVirtual.DelayedCall(0.15f, () => _isPopping = false);
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
