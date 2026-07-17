using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardRetriever
{
    private List<Card> currentDeck;

    public CardRetriever(List<Card> currentDeck)
    {
        this.currentDeck = currentDeck;
    }

    //Retrieves card to overpower player card in lane if possible, otherwise retrieves next card in deck
    public Card RetrieveCard(AIPersonality profile, LaneSnapShot lane)
    {
        Card bestCard = null;
        float highestCardScore = Mathf.NegativeInfinity;

        foreach(Card card in currentDeck)
        {
            if(card == null)
            {
                Debug.LogWarning($"Card being accessed is null");
                continue;
            }
            float attackWeight = card.BaseDamage * profile.AggressionMultipler;
            float healthWeight = card.BaseHealth * profile.SurvivalMultiplier;
            float finalWeight = attackWeight + healthWeight;

            if (lane.PlayerCard.HasValue && card.BaseHealth > lane.PlayerCard.Value.Attack)
            {
                if(finalWeight > highestCardScore)
                {
                    highestCardScore = finalWeight;
                    bestCard = card;
                }
            }
        }
        if(bestCard != null)
        {
            Debug.Log($"<color=cyan> This Card '{bestCard.name}' has enough health to survive the player's attack. Returning this card.</color>");
            return bestCard;
        }
        return RetrieveCard();
    }

    //Take the first card in the enemy deck. Use last.
    public Card RetrieveCard()
    {
        foreach(Card card in currentDeck)
        {
            if (card != null) return card;
        }
        return null;
    }
}
