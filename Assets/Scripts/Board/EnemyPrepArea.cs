using DG.Tweening;
using System;
using UnityEngine;

public class EnemyPrepArea : MonoBehaviour, ICardDropArea
{
    [SerializeField] private CardDropArea _frontCardDropArea;
    [SerializeField] private LaneView _parentLaneView;
    public Transform _cardSpawnLocation;

    public bool HasCard => _currentCard != null;
    private Card _currentCard;

    public event Action<Card, LaneView, Action> pushedCard;

    private void Start()
    {
        _parentLaneView = GetComponentInParent<LaneView>();
    }

    public void OnCardDrop(Card card)
    {
        //Animation
        _currentCard = card;
    }
    public void LoadCardAreas()
    {

    }
    public void TriggerPush(Action onCompleteCallback)
    {
        if (_currentCard == null) return;
        Card cardToPlay = _currentCard;
        _currentCard = null;
        pushedCard?.Invoke(cardToPlay, _parentLaneView, onCompleteCallback);
    }
}
