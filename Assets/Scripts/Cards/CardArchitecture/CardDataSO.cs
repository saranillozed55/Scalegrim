using UnityEngine;

[CreateAssetMenu(fileName = "CardDataSO")]
public class CardDataSO : ScriptableObject
{
    [field:SerializeField] public int Cost { get; private set; }
    [field:SerializeField] public int Health { get; private set; }
    [field:SerializeField] public int AttackDamage { get; private set; }
    [field:SerializeField] public string Name { get; private set; }
}
