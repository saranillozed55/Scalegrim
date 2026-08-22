using UnityEngine;
using Zeedo.LS.Map.Nodes;

namespace Zeedo.LS.Map.Generator
{
    public class MapGenerator
    {
        public void CreateMap()
        {
            //practicing
            MapNode enemy1 = new MapNode();
            MapNode campfire1 = new MapNode();
            MapNode enemy2 = new MapNode();
            enemy1.nodeLayer = 0;
            enemy1.neighbors.Add(campfire1);
            enemy1.neighbors.Add(enemy2);

//writing 
        }
    }
}
