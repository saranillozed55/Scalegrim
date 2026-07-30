using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnQueue
{
    private Queue<BlueprintTurn> _queue = new();

    public IReadOnlyCollection<BlueprintTurn> blueprintTurns;

    public EnemyTurnQueue()
    {
        blueprintTurns = _queue;
    }

    public void GenerateEnemyQueue(Blueprint blueprint)
    {
        foreach(BlueprintTurn entries in blueprint.Turns)
        {
            _queue.Enqueue(entries);
        }
        
    }
}
