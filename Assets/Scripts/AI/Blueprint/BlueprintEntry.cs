using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable] 
public class BlueprintEntry
{
    public EntryType type;

    public CardDataSO card;
}

public enum EntryType
{
    ExactCard,
    RandomCost,
    RandomTribe, // probably won't be using RandomTribe, maybe random something else(RandomSchool?)
    RandomAny
}
