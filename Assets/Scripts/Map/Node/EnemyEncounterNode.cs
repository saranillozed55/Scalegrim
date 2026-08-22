using System.Collections.Generic;
using UnityEngine;
using Zeedo.LS.Map.Nodes;
using Zeedo.LS.Encounter.Commmand;
using Zeedo.LS.Encounter.Interfaces;
using UnityEngine.EventSystems;

namespace Zeedo.LS.Map.Nodes
{
    public class EnemyEncounterNode : MonoBehaviour, IClickable
    {

        public void OnClick()
        {
            //To load I believe we get stuff from MapNodeData and some script process all that data and loads it 
            //Want to load EnemyEncounter and update MapNode
        }
    }
}
