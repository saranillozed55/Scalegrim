using System.Linq;
using UnityEngine;
public class CardModel
{
    private readonly CardDataSO cardData;

    public bool CardPlaced { get; private set; }
    public bool CardSelected { get; private set; }
    public bool CardHoverable { get; private set; }

    public void SetHoverable(bool value) => CardHoverable = value;
    public Lane Lane => BoardLaneManager.Instance.LogicLanes.FirstOrDefault(lane =>
            lane.PlayerActiveCard == this ||
            lane.EnemyActiveCard == this ||
            lane.EnemyQueuedCard == this
        );


    public Owner? BoardOwner
    {
        get
        {
            var lane = Lane;
            if (lane == null) return null;
            if (lane.PlayerActiveCard == this) return Owner.Player;
            if (lane.EnemyActiveCard == this) return Owner.Enemy;
            if (lane.EnemyActiveCard == this) return Owner.Enemy;
            return null;
        }
    }

    public bool IsOnBoard => Lane != null;
    public bool IsQueued => Lane?.EnemyQueuedCard == this;

    public CardModel(CardDataSO cardData)
    {
        this.cardData = cardData;
        Cost = cardData.Cost;
        Health = cardData.Health;
        AttackDamage = cardData.AttackDamage;
        ViewPrefab = cardData.ViewPrefab;
    }

    public string Name { get => cardData.Name; }
    public CardView ViewPrefab
    {
        get
        {
            return cardData.ViewPrefab;
        }
        private set
        {

        }
    }

    public int Cost { get; set; }
    public int Health { get; set; }
    public int AttackDamage { get; set; }
    public bool Dead { get; set; }


    public void PlayCard()
    {
        if (!CardPlaced)
            CardPlaced = true;
    }

    public void CardAttack(CardModel attackingCard, CardModel defendingCard)
    {
        CardsDamager.ApplyDamage(attackingCard, defendingCard);
    }
}