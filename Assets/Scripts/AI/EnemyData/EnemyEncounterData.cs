using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Encounter Data")]
public class EnemyEncounterData : ScriptableObject
{
    public string enemyID;
    public List<Blueprint> blueprints;

    //Later: Can add if the blueprint has been previously used for the previous run then can skip that
}
