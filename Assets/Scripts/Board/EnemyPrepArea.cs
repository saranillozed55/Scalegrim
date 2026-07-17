using DG.Tweening;
using System;
using UnityEngine;

public class EnemyPrepArea : MonoBehaviour, ICardDropArea
{
    [SerializeField] private CardDropArea _frontCardDropArea;
    [SerializeField] private LaneView _parentLaneView;
    public Transform _cardSpawnLocation;

    public bool HasCard => _currentCard != null;
    public CardDropArea FrontCardDropArea => _frontCardDropArea;

    private Card _currentCard;

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
    public Card TriggerPush()
    {
        if (_currentCard == null) return null;
        Card cardToPlay = _currentCard;
        _currentCard = null;
        return cardToPlay;
    }
}
