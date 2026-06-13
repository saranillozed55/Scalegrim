using TMPro;
using UnityEngine;

public class CardUI : MonoBehaviour
{
    private Card _card;
    [Header("UI")]
    [SerializeField] private TMP_Text _cardHealthText;

    private void Awake()
    {
        _card = GetComponent<Card>();
        _cardHealthText.text = _card.CardData.health.ToString();
    }
    
}
