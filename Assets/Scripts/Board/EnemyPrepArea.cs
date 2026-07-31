using DG.Tweening;
using System;
using UnityEngine;

public class EnemyPrepArea : MonoBehaviour, ICardDropArea
{
    [SerializeField] private CardDropArea _frontCardDropArea;
    public Transform _cardSpawnLocation;

    public bool HasCard => _currentCard != null;
    public CardDropArea FrontCardDropArea => _frontCardDropArea;

    private CardView _currentCard;

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
