using System.Collections.Generic;
using UnityEngine;
using Zeedo.LS.Map.Nodes;
using Zeedo.LS.Encounter.Commmand;
using Zeedo.LS.Encounter.Interfaces;

namespace Zeedo.LS.Map.Nodes
{
    public class EnemyEncounterNode : MapNodeDataSO
    {

        [SerializeField] private List<Blueprint> blueprints;

        public override IEncounter CreateEncounter()
        {
            return new EnemyEncounterCommand();
        }
    }

    
}
