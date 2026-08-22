using System.Collections.Generic;
using Zeedo.LS.Encounter.Enums;

namespace Zeedo.LS.Map.Nodes
{
    public class MapNode
    {
        public int nodeLayer;
        public bool visited;
        public MapNodeData nodeData;
        public List<MapNode> neighbors;
    }

    //MapNode had MapNodeData and then these are created in MapGenerator. 
    //Still have to think about how to create it so when we click on it then thats the encounter it will load
}
