using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    [SerializeField] private int _maxStack = 5;
    private List<CardModel> _deck = new();
    public List<CardModel> _startingDeck = new(); 

    public List<CardModel> Deck => _deck;

    private void Start()
    {
        InitalizeBaseDeck();
    }

    private void Update()
    {

    }

    private void InitalizeBaseDeck()
    {
        for (int i = 0; i < _maxStack; i++)
        {
            _deck.Add(_startingDeck[i]);
        }
    }

    public void AddToPlayerDeck(CardModel card)
    {
        if(card != null)
        {
            _deck.Add(card);
        }
    }

    public void RemoveToPlayerDeck(CardModel card)
    {
        if(_deck.Contains(card))
        {
            _deck.Remove(card);
        }
    }
}
