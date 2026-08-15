using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public struct BoardState
{
    public List<LaneSnapShot> LanesShot;
}

public struct LaneSnapShot
{
    public int LaneIndex;
    public CardSnapShot? EnemyCard;
    public CardSnapShot? PlayerCard;
    public CardSnapShot? EnemyQueuedCard;
}

public struct CardSnapShot
{
    public CardModel cardModel;
}
