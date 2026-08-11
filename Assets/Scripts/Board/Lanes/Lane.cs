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

    public CardDropAreaData PlayerArea { get; }
    public CardDropAreaData EnemyArea { get; }

    public Lane(int laneIndex, EnvironmentType playerEnvironment, EnvironmentType enemyEnvironment)
    {
        LaneIndex = laneIndex;

        PlayerArea = new CardDropAreaData(playerEnvironment);
        EnemyArea = new CardDropAreaData(enemyEnvironment);
    }
}
