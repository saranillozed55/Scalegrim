using UnityEngine;

public class Player : MonoBehaviour
{
    public static readonly float CurrentPlayerHealth = 15f; // this should be moved to data
    private PlayerData playerData;


    public void Initialize(PlayerData data) // this should be used for reading off of json later
    {
        playerData = data;
        playerData.health = data.health;
    }

    public void TakeDamage(int damageAmount)
    {
        playerData.health = Mathf.Max(0, playerData.health - damageAmount);
    }



    public PlayerData GetPlayerData()
    {
        return playerData;
    }
}
