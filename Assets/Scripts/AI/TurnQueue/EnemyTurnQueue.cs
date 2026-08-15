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
        foreach (BlueprintTurn entries in blueprint.Turns)
        {
            _queue.Enqueue(entries);
        }
    }

    public BlueprintTurn GetTurnBlueprint()
    {
        if(_queue.Count == 0)
        {
            Debug.Log("There is not more BlueprintTurns in queue");
            return null;
        }
        return _queue.Dequeue();
    }
}
