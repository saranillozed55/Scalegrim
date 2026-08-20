using System.Collections.Generic;
using UnityEngine;

//Rename this script into LaneEncounterData?
public class EncounterData 
{
    private readonly List<Lane> _lanes = new();
    public IReadOnlyCollection<Lane> lanes => _lanes;

    public EncounterData()
    {
        //testing... Don't hard code these. Must change these probably before player starts thier run
        _lanes.Add(new Lane(0, EnvironmentType.Land, EnvironmentType.DeepWaters));
        _lanes.Add(new Lane(1, EnvironmentType.ShallowWaters, EnvironmentType.Abyss));
        _lanes.Add(new Lane(2, EnvironmentType.DeepWaters, EnvironmentType.DeepWaters));
        _lanes.Add(new Lane(3, EnvironmentType.Abyss, EnvironmentType.Abyss));
    }

    public Lane GetLane(int laneIndex)
    {
        return _lanes[laneIndex];
    }
}
