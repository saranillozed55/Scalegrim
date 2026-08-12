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
            Debug.Log("This should damage enemy/person");
            CardEventBus.RaiseOnDirectDamage(damage);
            //CombatManager.Instance.PlayerTakeDamage(damage);
        }
        else
        {
            defendingCard.TakeDamage(damage);
        }
    }
}
