
using Zeedo.LS.Encounter.Interfaces;
using UnityEngine;

namespace Zeedo.LS.Map.Nodes
{
    public abstract class MapNodeDataSO : ScriptableObject
    {
        [Header("Map visual")]
        public Sprite nodeIcon;
        public string nodeName;

        public abstract IEncounter CreateEncounter();
    }
}
