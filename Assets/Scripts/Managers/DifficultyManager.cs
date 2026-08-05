using UnityEngine;

public class DifficultyManager : GenericSingleton<DifficultyManager>
{

    public int DifficultyLevel { get; private set; } = 1;

    public void SetDifficultyLevel(int level)
    {
        DifficultyLevel = level;
    }

    public void IncreaseDifficultyLevel()
    {
        DifficultyLevel++;
    }

    public void ResetDifficultyLevel()
    {
        DifficultyLevel = 1;
    }

}
