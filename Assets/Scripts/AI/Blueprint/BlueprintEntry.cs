using System;
using System.Collections.Generic;
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
    RandomCost, // random cost card from a specific group( or not)
    RandomGroup, // random card from a specific group
    RandomFromAny, // random card from list of cards
}
