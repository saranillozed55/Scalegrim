using System;
using UnityEngine;

[Serializable]
public class CardDropAreaData
{
    public AreaOwnerType SlotOwner { get; private set; }
    public EnvironmentType Environment { get; private set; }
    public int LaneIndex { get; private set; }
    public bool IsTaken { get; private set; }
    public bool IsHidden { get; private set; } // use this to hide the area when we are not fighting ?

    //public event Action<EnvironmentType> OnEnvironmentChanged;

    public CardDropAreaData(EnvironmentType environment)
    {
        Environment = environment;
    }

    public void ChangeEnvironmentType(EnvironmentType environment)
    {
        Environment = environment; // make sure to update visuals
    }

}
