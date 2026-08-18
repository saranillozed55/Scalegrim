using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

/*
 * This should just be called then whenever we start a combat encounter, probably in EncounterManager
 */
public class EnemyEncounter : IDamageable
{
    private readonly string enemyID; // Encounters will have ID and that ID is tied to what blueprint they have. Thinking maybe encounters should be SO's?
    private readonly List<Blueprint> blueprints;
    private readonly CardPlacer cardPlacer;
    private readonly EnemyTurnQueue enemyTurnQueue;
    private readonly BlueprintRetriever blueprintRetriever;
    private readonly CardGroupRetriever cardGroupRetriever;
    private readonly EnemyEncounterData enemyEncounterData;

    private bool hasSurrendered = false;
    private int Health;

    public event System.Action OnFinishedTurn;

    //TODO: Work on Enemy
    // Encounter Data should have the list of the blueprints and health probably will be like inscryption where we must get over 5 damage against the enemy
    public EnemyEncounter(EnemyEncounterData enemyEncounterData, int health, BlueprintRetriever blueprintRetriever, CardGroupRetriever cardGroupRetriever)
    {
        cardPlacer = new CardPlacer();
        enemyTurnQueue = new EnemyTurnQueue();
        this.enemyEncounterData = enemyEncounterData;
        this.blueprintRetriever = blueprintRetriever;
        this.cardGroupRetriever = cardGroupRetriever;
        blueprints = enemyEncounterData.blueprints;
        enemyID = enemyEncounterData.enemyID;
        Health = health;

        blueprintRetriever.SetBlueprintsByDifficulty(blueprints);
    }

    public bool HasSurrendered => hasSurrendered;

    public void TakeDamage(int damage)
    {
        Health -= damage;
    }

    public void OnEncounterStart()
    {
        if (blueprints.Count <= 0)
        {
            Debug.Log("No blueprint was given to the enemy");
        }

        Blueprint blueprint = blueprintRetriever.GetBlueprintByDifficultyAndRandom(2); // placeholder 1;
        Debug.Log($"Blueprint chosen: {blueprint.name}");
        enemyTurnQueue.GenerateEnemyQueue(blueprint);

        //should go to TurnPerformed
        OnPrepareNextTurnHandler();
    }

    public async void OnPrepareNextTurnHandler()
    {
        await QueueNextCards();
    }

    public virtual async Task<bool> QueueNextCards()
    {
        BlueprintTurn blueprintTurn = enemyTurnQueue.GetTurnBlueprint();

        if (blueprintTurn == null) // can also make it so that we can trick the player into thinking enemy has surrendered
        {
            Debug.Log("Enemy has no more turns, he will surrender");
            SurrenderPerformed();
            return false;
        }


        List<int> availableLanes = BoardLaneManager.Instance.GetAvailableEnemyLanes();

        foreach (BlueprintEntry entry in blueprintTurn.Entries)
        {
            //do the preference here too

            EntryType type = entry.type;
            EnemyAttackPreference preference = entry.enemyAttackPreference;

            //HERE: If there is a preference should reference other method and continue

            int randomIndex = Random.Range(0, availableLanes.Count);
            int lane = availableLanes[randomIndex];

            bool wasPlaced = false;


            if (type == EntryType.ExactCard)
            {
                CardModel model = new CardModel(entry.card);
                wasPlaced = await cardPlacer.HandlePlaceCard(model, lane);
            }

            else if (type == EntryType.RandomGroup)
            {
                List<CardDataSO> cards = cardGroupRetriever.GetCardsByGroup(entry.group);
                CardDataSO randomCard = cards[Random.Range(0, cards.Count)];
                CardModel model = new CardModel(randomCard);
                
                wasPlaced = await cardPlacer.HandlePlaceCard(model, lane);
            }

            else if (type == EntryType.RandomFromAny)
            {
                if (entry.cardListFromAny == null || entry.cardListFromAny.Count == 0)
                {
                    continue;
                }
                List<CardDataSO> cards = cardGroupRetriever.GetCardsFromAnyInList(entry.cardListFromAny);
                CardDataSO randomCard = cards[Random.Range(0, cards.Count)];
                CardModel model = new CardModel(randomCard);

                wasPlaced = await cardPlacer.HandlePlaceCard(model, lane);
            }

            if (wasPlaced)
            {
                availableLanes.RemoveAt(randomIndex);
            }
        }
        return true;
    }

    public virtual void SurrenderPerformed() //don't know if this should be virtual or not
    {
        Debug.Log("Surrender performed");
        hasSurrendered = true;
    }


    public bool HasLost()
    {
        return Health <= 0;
    }

}
