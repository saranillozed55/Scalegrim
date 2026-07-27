using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardRetriever
{
    private List<CardModel> currentDeck;

    public CardRetriever(List<CardModel> currentDeck)
    {
        this.currentDeck = currentDeck;
    }

    //Retrieves card to overpower player card in lane if possible, otherwise retrieves next card in deck
    public CardModel RetrieveCard(AIPersonality profile, LaneSnapShot lane)
    {
        CardModel bestCard = null;
        float highestCardScore = Mathf.NegativeInfinity;

        foreach(CardModel card in currentDeck)
        {
            if(card == null)
            {
                Debug.LogWarning($"Card being accessed is null");
                continue;
            }
            float attackWeight = card.AttackDamage * profile.AggressionMultipler;
            float healthWeight = card.Health * profile.SurvivalMultiplier;
            float finalWeight = attackWeight + healthWeight;

            if (lane.PlayerCard.HasValue && card.Health > lane.PlayerCard.Value.Attack)
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
            Debug.Log($"<color=cyan> This Card '{bestCard.Name}' has enough health to survive the player's attack. Returning this card.</color>");
            return bestCard;
        }
        return RetrieveCard();
    }

    //Take the first card in the enemy deck. Use last.
    public CardModel RetrieveCard()
    {
        foreach(CardModel card in currentDeck)
        {
            if (card != null) return card;
        }
        return null;
    }
}
