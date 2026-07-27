using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "Card Database")]
public class CardDatabaseSO : ScriptableObject
{
    [SerializeField] private List<CardDataSO> _allCards = new();

    public CardDataSO GetCardById(string cardId)
    {
       return _allCards.FirstOrDefault(c => c.Id == cardId);
    }
}
