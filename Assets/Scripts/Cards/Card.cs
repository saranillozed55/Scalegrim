using DG.Tweening;
using UnityEngine;

public class Card : MonoBehaviour, IHoverable, IClickable
{
    [Header("BroadCast Event Channels")]
    [SerializeField] private CardEventChannelSO _cardClicked;
    private bool _isSelected = false;
    private const int _placedCardLayer = 6;

    public Vector3 _basePosition;
    public Quaternion _baseRotation;
    public bool _cardIsPlaced = false;

    public void CardIsPlayed()
    {
        _cardIsPlaced = true;
        gameObject.layer = _placedCardLayer;
        foreach(Transform child in transform)
        {
            child.gameObject.layer = _placedCardLayer;
        }
    }

    public virtual void OnHoverEnter()
    {
        if (_cardIsPlaced) return;
        transform.DOKill(); // Stop any ongoing tweens to prevent conflicts
        transform.DOMove(_basePosition + Vector3.up * 0.1f, 0.2f);
        transform.DORotateQuaternion(_baseRotation, 0.25f);
    }
    public virtual void OnHoverExit()
    {
        if (_cardIsPlaced) return;
        transform.DOKill(); // Stop any ongoing tweens to prevent conflicts
        transform.DOMove(_basePosition, 0.25f);
        transform.DORotateQuaternion(_baseRotation, 0.25f);
    }

    public void OnClick()
    {
        if (_isSelected || _cardIsPlaced) return;
        // Implement card click behavior here
        _cardClicked.RaiseEvent(this);
        Debug.Log($"Card {gameObject.name} clicked!");
    }
}
