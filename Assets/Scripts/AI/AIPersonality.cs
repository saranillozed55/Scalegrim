using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[CreateAssetMenu(fileName = "AI", menuName = "AI/Personality")]
public class AIPersonality : ScriptableObject
{

    [Header("Personality Profile")]
    public string EnemyName;

    [Header("Core Multipliers")]
    [Tooltip("How much the AI values dealing direct damage. High = More Aggressive")]
    public float AggressionMultipler = 1.5f;

    [Tooltip("How much the AI fears incoming play damage. High = More defensive")]
    public float DefensiveMultiplier = 1.2f;

    [Tooltip("How much the AI values its own card's health over its attack stats.")]
    public float SurvivalMultiplier = 1.0f;

    [Header("Flat Bonuses")]
    public float KillPlayerBonus = 25f;
    public float BlockPlayerBonus = 5f;
    public float EmptyLanePenalty = -2f;
}
