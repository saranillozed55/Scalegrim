using UnityEngine;

public class CardsDamager
{

    private Card attackingCard;
    private Card targetCard;

    public CardsDamager(Card attackingCard, Card targetCard)
    {
        this.attackingCard = attackingCard;
        this.targetCard = targetCard;
    }

    public void ApplyDamage()
    {
        if (targetCard == null)
        {
            Debug.Log("This should damage enemy/person");
            return;
        }
        else
        {
            targetCard._cardData._health -= attackingCard._cardData._attackDamage;
            CheckCardDeath();

            Debug.Log($"Damage done to target card {attackingCard.DamageCurrent}, Opposite Card Health: {targetCard.HealthCurrent}");
        }
    }

    private void CheckCardDeath()
    {
        if(targetCard._cardIsPlaced && targetCard.HealthCurrent <= 0 && !targetCard.IsCardDead)
        {
            targetCard.IsCardDead = true;
            GameObject.Destroy(targetCard.gameObject);
        }
    }
}
