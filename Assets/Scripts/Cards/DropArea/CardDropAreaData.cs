using System;
using UnityEngine;

[Serializable]
public class CardDropAreaData
{
    public AreaOwnerType SlotOwner { get; private set; }
    public LaneEnvironment Environment { get; private set; }
    public int LaneIndex { get; private set; }
    public bool IsTaken { get; private set; }
    public bool IsHidden { get; private set; } // use this to hide the area when we are not fighting ?

    public CardDropAreaData(int laneIndex, AreaOwnerType areaOwnerType, LaneEnvironment environment)
    {
        LaneIndex = laneIndex;
        SlotOwner = areaOwnerType;
        Environment = environment;
    }

    public void ChangeEnvironmentType(LaneEnvironment environment)
    {
        Environment = environment; // make sure to update visuals
    }

}
