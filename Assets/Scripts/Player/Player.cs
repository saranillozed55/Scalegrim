using Cards.Events;
using UnityEngine;

namespace MainPlayer
{
    public class Player : MonoBehaviour, IDamageable
    {
        private PlayerData playerData;
        public int Health { get; private set; }

        private void Awake()
        {
            Health = 15; // temporary
        }

        private void OnEnable()
        {
            CardEventBus.OnDirectDamage += TakeDamage;
        }

        private void OnDisable()
        {
            CardEventBus.OnDirectDamage -= TakeDamage;
        }

        public void Initialize(PlayerData data) // this should be used for reading off of json later
        {
            playerData = data;
            Health = playerData.health;
        }

        public void TakeDamage(int damageAmount)
        {
            Health = Mathf.Max(0, Health - damageAmount);
            Debug.Log($"Damage taken: {damageAmount}, HP left {Health}");
        }

        public PlayerData GetPlayerData()
        {
            return playerData;
        }
    }
}
