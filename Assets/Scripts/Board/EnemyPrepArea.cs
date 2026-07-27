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

    private CardView _currentCard;

    private void Start()
    {
        _parentLaneView = GetComponentInParent<LaneView>();
    }

    public void OnCardDrop(CardView card)
    {
        //Animation
        _currentCard = card;
    }
    public void LoadCardAreas()
    {

    }
    public CardView TriggerPush()
    {
        if (_currentCard == null) return null;
        CardView cardToPlay = _currentCard;
        _currentCard = null;
        return cardToPlay;
    }
}
