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

    private Stack<GameObject> _deckCards = new();

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
    public void LoadDeck(List<Card> deckCards)
    {
        for (int i = 0; i < deckCards.Count; i++)
        {
            GameObject instance = Instantiate(deckCards[i].gameObject, _spawnLocation.position, _spawnLocation.rotation);

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
        GameObject poppedCard = _deckCards.Pop();
        HandManager.Instance.DrawCard(poppedCard);
    }

    public void ClearDeckStack()
    {
        if (_deckCards.Count > 0)
        {
            foreach (GameObject card in _deckCards)
            {
                Destroy(card);
            }
            _deckCards.Clear();
            _stackLoaded = false;
        }
    }
}
