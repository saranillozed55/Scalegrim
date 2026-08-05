using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable] 
public class BlueprintEntry
{

    public CardDataSO card;
    public EntryType type;
    public Group group; // use for random group or random cost
    public EnemyAttackPreference enemyAttackPreference; // use for any

    [Tooltip("Use for random cost or random from any")]
    public List<CardDataSO> cardListFromAny; 
}

public enum EntryType
{
    ExactCard, 
    RandomCost, // random cost card from a specific group ( or not)
    RandomGroup, // random card from a specific group
    RandomFromAny, // random card from list of cards
}
