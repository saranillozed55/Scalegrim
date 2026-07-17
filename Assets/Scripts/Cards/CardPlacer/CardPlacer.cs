using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CardPlacer
{
    public void HandlePlaceCard(List<Card> enemyDeck, Card card, int laneIndex) // maybe change enemyDeck to pass in an object rather than list then in that class we can call method to remove the card. but for now is fine
    {
        if (card == null)
        {
            Debug.LogWarning($"HandlePlaceCard: Recieved null card, can't place card in Queue");
            return;
        }

        BoardLaneManager.Instance.PlaceEnemyCardsInQueue(card, laneIndex, out bool full);

        if (!full)
        {
            enemyDeck.Remove(card);
        }
        else
        {
            Debug.Log($"<color=cyan> This Card can't be placed in queue because there is already a card in the active area or the prep area. </color>");
        }
    }
}
