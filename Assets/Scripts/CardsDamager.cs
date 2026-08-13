using UnityEditor;
using UnityEngine;
using Cards.Events;

//this will be used for calculations as well once game gets bigger
public static class CardsDamager
{
    public static void ApplyDamage(CardModel attackingCard, CardModel defendingCard)
    {
        int damage = attackingCard.AttackDamage;
        if (defendingCard == null)
        {
            var owner = attackingCard.BoardOwner;

            if(owner != null)
            {
                if(owner == AreaOwnerType.PlayerActive) // should do damage to enemy
                {
                    CardEventBus.RaiseOnDirectEnemyDamage(damage);
                }
                else if(owner == AreaOwnerType.EnemyActive) // should do damage to player
                {
                    CardEventBus.RaiseOnDirectPlayerDamage(damage);
                }
            }
        }
        else
        {
            defendingCard.TakeDamage(damage);
        }
    }
}
