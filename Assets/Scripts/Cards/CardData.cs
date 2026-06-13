using UnityEngine;

[CreateAssetMenu(fileName = "CardData/CardData")]
public class CardData : ScriptableObject
{

    public SpriteRenderer spriteRenderer;
    //public Image subIcon; // icon for if a card has like a passive or special ability and image 
    public int damage;
    public int health;
    public bool isDead;

}
