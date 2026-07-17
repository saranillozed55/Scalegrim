using UnityEngine;
public class CardModel
{
    private readonly CardDataSO cardData;

    public CardModel(CardDataSO cardData)
    {
        this.cardData = cardData;
        Cost = cardData.Cost;
        Health = cardData.Health;
        AttackDamage = cardData.AttackDamage;
    }
    public int Cost { get; set; }
    public int Health { get; set; }
    public int AttackDamage { get; set; }
    public string Name { get => cardData.Name; }
}