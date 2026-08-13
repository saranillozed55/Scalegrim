using UnityEngine;

public class GameplayBehaviourManager : GenericSingleton<GameplayBehaviourManager>
{
    public bool GameplayInputEnabled { get; private set; } = true;

    private void Start()
    {
        //Debug.Log($"{this}: {GameplayInputEnabled}");
    }

    public void EnableGameplay()
    {
        GameplayInputEnabled = true;
    }
    public void DisableGameplay()
    {
        GameplayInputEnabled = false;
    }
}
