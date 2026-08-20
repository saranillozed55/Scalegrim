using UnityEngine;
using Zeedo.LS.Map.Nodes;

public class MapNode : MonoBehaviour
{
    //this will be initialized once player sits down to play
    [SerializeField] private MapNodeDataSO mapNodeDataSO;

    //private MapNodeDataSO mapNodeDataSO;

    public void Init(MapNodeDataSO mapNodeDataSO)
    {
        this.mapNodeDataSO = mapNodeDataSO;
    }

    public void Selected()
    {
        //starts the encounter once clicked
        mapNodeDataSO.CreateEncounter();
    }
}
