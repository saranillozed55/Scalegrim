using UnityEngine;

public static class CardsDamager
{
    //public static void ApplyDamage(Card attackingCard, Card targetCard)
    //{
    //    if (targetCard == null)
    //    {
    //        Debug.Log("This should damage enemy/person");
    //        return;
    //    }
    //    else
    //    {
    //        targetCard._cardData._health -= attackingCard._cardData._attackDamage;
    //        CheckCardDeath(targetCard);

    //        Debug.Log($"Damage done to target card {attackingCard.DamageCurrent}, Opposite Card Health: {targetCard.HealthCurrent}");
    //    }
    //}

    public static void ApplyDamage(CardModel attackingCard, CardModel defendingCard)
    {
        if (defendingCard == null)
        {
            Debug.Log("This should damage enemy/person");
            return;
        }
        else
        {
            defendingCard.Health -= attackingCard.AttackDamage;
            CheckCardDeath(defendingCard);

            Debug.Log($"Damage done to target card {attackingCard.AttackDamage}, Opposite Card Health after Damage: {defendingCard.Health}");
        }
    }

    private static void CheckCardDeath(CardModel defendingCard)
    {
        if(defendingCard.CardPlaced && defendingCard.Health <= 0 && !defendingCard.Dead)
        {
            defendingCard.Dead = true;
        }
    }
}
