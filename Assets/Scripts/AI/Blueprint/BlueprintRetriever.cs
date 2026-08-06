using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "BlueprintRetriever")]
public class BlueprintRetriever : ScriptableObject
{
    private Dictionary<int, List<Blueprint>> _bluePrintsByDifficulty;

    private void OnEnable()
    {
        if(_bluePrintsByDifficulty != null)
        {
            _bluePrintsByDifficulty.Clear(); //not sure if we clear this at onEnable or not, but we should clear it at some point to avoid memory leaks
        }
        _bluePrintsByDifficulty = new();
    }

    public void SetBlueprintsByDifficulty(List<Blueprint> blueprints) //must set blueprints before calling GetBlueprintsByDifficulty or GetBlueprintByDifficultyAndRandom
    {
        _bluePrintsByDifficulty = blueprints.GroupBy(blueprint => blueprint.difficultyLevelOfBlueprint).ToDictionary(group => group.Key, group => group.ToList());
        
        if(_bluePrintsByDifficulty.Count == 0)
        {
            Debug.LogWarning("No blueprints found for the given difficulty levels.");
        }
    }

    //SetBlueprints here as well to check if blueprints match the card pre-requisites 
    //Set or Get not sure yet, but we need to check if the blueprints match the card pre-requisites before we can use them

    public List<List<Blueprint>> GetBlueprintsByDifficulty(int difficultyLevel)
    {
        return _bluePrintsByDifficulty.Where(blueprint => blueprint.Key == difficultyLevel).Select(blueprint => blueprint.Value).ToList();
    }

    public List<Blueprint> GetBlueprintsByDifficultyWithRandom(int difficultyLevel)
    {
        List<List<Blueprint>> blueprints = GetBlueprintsByDifficulty(difficultyLevel);
        if (blueprints.Count > 0)
        {
            return blueprints[Random.Range(0, blueprints.Count)];
        }
        return new List<Blueprint>();
    }

    public Blueprint GetBlueprintByDifficultyAndRandom(int difficultyLevel)
    {
        List<Blueprint> blueprints = GetBlueprintsByDifficultyWithRandom(difficultyLevel);
        if (blueprints.Count > 0)
        {
            int random = Random.Range(0, blueprints.Count);
            return blueprints[random];
        }
        return null;
    }
}
