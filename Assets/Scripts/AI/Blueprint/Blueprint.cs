
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Blueprint", menuName = "AI/Blueprint")]
public class Blueprint : ScriptableObject
{
    public List<BlueprintTurn> Turns;

    public int difficultyLevelOfBlueprint = 1;    

}
