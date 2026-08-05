using UnityEngine;

[CreateAssetMenu(fileName = "CardDataSO")]
public class CardDataSO : ScriptableObject
{
    [field: SerializeField] public CardView ViewPrefab { get; private set; }
    [field:SerializeField] public string Id { get; private set; }
    [field:SerializeField] public string Name { get; private set; }
    [field:SerializeField] public int Cost { get; private set; }
    [field:SerializeField] public int Health { get; private set; }
    [field:SerializeField] public int AttackDamage { get; private set; }
    [field: SerializeField] public Group Group { get; private set; }

    [field:SerializeField] public CardAudioProfileSO AudioProfile { get; private set; }

}
