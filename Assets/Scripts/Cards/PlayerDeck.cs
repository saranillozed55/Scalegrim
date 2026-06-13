using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

//Can have ScriptableObject for PlayerDeck maybe ? not sure yet
public class PlayerDeck : MonoBehaviour
{
    [SerializeField] private Card _tempCardPrefab;
    [SerializeField] private int _maxStack = 5;
    private List<Card> _deck = new();
    public List<Card> _startingDeck = new(); // Use inspector

    public List<Card> Deck => _deck;

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
            //Debug.Log("Added card: " + _tempCardPrefab.name);
        }
    }

    public void AddToPlayerDeck(Card card)
    {
        if(card != null)
        {
            _deck.Add(card);
        }
    }

    public void RemoveToPlayerDeck(Card card)
    {
        if(_deck.Contains(card))
        {
            _deck.Remove(card);
        }
    }
}
