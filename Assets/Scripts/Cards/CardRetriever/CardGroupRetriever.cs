using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CardGroupRetriever")]
public class CardGroupRetriever : ScriptableObject
{
    [SerializeField] public CardDatabaseSO _dataBase;

    private Dictionary<Group, List<CardDataSO>> _cardsByGroup;

    private void OnEnable()
    {
        _cardsByGroup.Clear();
        _cardsByGroup = new();

        _cardsByGroup = _dataBase.AllCards.GroupBy(card => card.Group).ToDictionary(group => group.Key, group => group.ToList());
        // GroupBy groups cards with the same result. Not yet a dictionary but a collection of groups. Ex:
        // Group 1 (Key = Land) - Cards(List): Turtle, Crab, Bear
        // Group 2 (Key = Water) - Cards(List): Fish, Shark, Octopus
        // etc.

        // Each group has a Key so group.Key and the value is the group.ToList() so it then creates the dictionary with the key and value.
    }

    //private List<CardDataSO> GetCardsByGroup(Group group)
    //{
        
    //}

}
