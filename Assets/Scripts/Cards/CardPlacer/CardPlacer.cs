using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CardPlacer
{
    public async Task HandlePlaceCard(List<CardModel> enemyDeck, CardModel card, int laneIndex) // maybe change enemyDeck to pass in an object rather than list then in that class we can call method to remove the card. but for now is fine
    {
        if (card == null)
        {
            Debug.LogWarning($"HandlePlaceCard: Recieved null card, can't place card in Queue");
            return;
        }

        bool placed = await BoardLaneManager.Instance.PlaceEnemyCardsInQueue(card, laneIndex);

        if (placed)
        {
            enemyDeck.Remove(card);
        }
        else
        {
            Debug.Log("This card can't be placed in queue because there is already a card in the active area");
        }
    }
}
