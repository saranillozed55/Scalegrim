using System;
using UnityEngine;

[Serializable]
public class Lane
{
    public int LaneIndex { get; set; }

    public Card PlayerActiveCard { get; set; }
    public Card EnemyActiveCard { get; set; }

    public Card EnemyQueuedCard { get; set; }
    public bool IsEnemySideOccupied => EnemyActiveCard != null;
    public bool IsPlayerSideOccupied => PlayerActiveCard != null;
    public bool IsQueueSlotOccupied => EnemyQueuedCard != null;
}
