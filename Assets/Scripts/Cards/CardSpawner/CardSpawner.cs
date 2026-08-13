using UnityEngine;

public class CardSpawner 
{    
    //maybe will have other methods that don't use the constructed properties
    public CardView SpawnDesignatedCard(CardView card, Transform spawnTransform, Transform parentTransform) 
    {
        return Object.Instantiate(card, spawnTransform.position, spawnTransform.rotation, parentTransform);
    }
}
