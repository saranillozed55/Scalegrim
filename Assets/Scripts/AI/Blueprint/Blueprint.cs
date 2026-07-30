
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Blueprint", menuName = "AI/Blueprint")]
public class Blueprint : ScriptableObject
{
    public List<BlueprintTurn> Turns;
}
