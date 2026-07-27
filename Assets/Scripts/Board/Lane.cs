using System;
using UnityEngine;

[Serializable]
public class Lane
{
    public int LaneIndex { get; set; }

    public CardModel PlayerActiveCard { get; set; }
    public CardModel EnemyActiveCard { get; set; }

    public CardModel EnemyQueuedCard { get; set; }
    public bool IsEnemySideOccupied => EnemyActiveCard != null;
    public bool IsPlayerSideOccupied => PlayerActiveCard != null;
    public bool IsQueueSlotOccupied => EnemyQueuedCard != null;
}
