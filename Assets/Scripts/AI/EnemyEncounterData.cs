using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Encounter Data")]
public class EnemyEncounterData : ScriptableObject
{
    public string enemyID;
    public List<Blueprint> blueprints;
}
