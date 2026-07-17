using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

//Want card view to be initialized when we load it into the game such as the player deck stack
public class CardView : MonoBehaviour, IClickable, IHoverable
{

    [Header("View References")]
    //[SerializeField] private SpriteRenderer _cardImage;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _cost;
    [SerializeField] private TMP_Text _health;

    private CardModel card;

    [Header("Enemy/Player Card Properties")]
    private bool _cardIsPlaced = false;

    [Header("Player Card Properties")]
    private bool _cardIsSelected = false;
    private bool _hoverable = false;
    private Vector3 _basePosition;
    private Quaternion _baseRotation;
    private Vector3 _placedPosition;

    public void SetBasePosition(Vector3 value) => _basePosition = value;
    public void SetBaseRotation(Quaternion value) => _baseRotation = value;
    public void SetPlacedPosition(Vector3 value) => _placedPosition = value;

    public void InitCard(CardModel card)
    {
        this.card = card;
        _name.text = card.Name;
        _cost.text = card.Cost.ToString();
        _health.text = card.Health.ToString();
    }

    public virtual void OnHoverEnter()
    {
        if (_cardIsPlaced || !_hoverable) return;
        transform.DOKill();
        transform.DOMove(_basePosition + Vector3.up * 0.1f + Vector3.back * 0.02f, 0.2f);
        transform.DORotateQuaternion(_baseRotation, 0.2f);
    }
    public virtual void OnHoverExit()
    {
        if (_cardIsPlaced || !_hoverable) return;
        transform.DOKill();
        transform.DOMove(_basePosition, 0.2f);
        transform.DORotateQuaternion(_baseRotation, 0.2f);
    }
    public virtual void OnClick()
    {
        if (_cardIsSelected || _cardIsPlaced || !_hoverable) return;

        
    }

}
