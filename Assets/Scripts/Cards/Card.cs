using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class Card : MonoBehaviour, IHoverable, IClickable
{
    private const int _placedCardLayer = 6;

    [Header("Card DataSO")] // SO so we can change text mesh pro easier
    [SerializeField] private CardData _cardData; // PROBLEM: If we have multiple of the same cards, the SO will be pointing to both of those so if they take damage they will both lose health

    [Header("BroadCast Event Channels")]
    [SerializeField] private CardEventChannelSO _cardClicked;
    [SerializeField] private CameraStateEventChannel _cardSelected;

    private bool _cardIsSelected = false;
    public Vector3 _basePosition;
    public Vector3 _placedPosition;
    public Quaternion _baseRotation;
    public bool _cardIsPlaced = false;

    public CardData CardData => _cardData;

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
        transform.DOMove(_basePosition + Vector3.up * 0.1f + Vector3.back * 0.02f, 0.2f);
        transform.DORotateQuaternion(_baseRotation, 0.25f);
    }
    public virtual void OnHoverExit()
    {
        if (_cardIsPlaced) return;
        transform.DOKill(); // Stop any ongoing tweens to prevent conflicts
        transform.DOMove(_basePosition, 0.25f);
        transform.DORotateQuaternion(_baseRotation, 0.25f);
    }

    public virtual void OnClick()
    {
        if (_cardIsSelected || _cardIsPlaced) return;

        // Implement card click behavior here
        _cardClicked.RaiseEvent(this);
        _cardSelected.RaiseEvent(CameraState.BoardCamera);

        Debug.Log($"Card {gameObject.name} clicked!");
    }

    //THIS IS JUST A PLACEHOLDER, SOME CARDS WON'T ATTACK IN GAME SO WOULD WANT TO AN IATTACKER INTERFACE FOR THE CARDS THAT DO ATTACK  
    public void PlayAttack(System.Action onImpact, System.Action onComplete, Vector3 attackDirection) //Call back parameter
    {

        transform.DOKill();
        transform.DOMove(_placedPosition + attackDirection * 0.3f, 0.15f).OnComplete(() =>
        {
            onImpact?.Invoke();

            transform.DOMove(_placedPosition, 0.15f).OnComplete(() => onComplete?.Invoke());
        });
    }

    public virtual void CheckCardDeath()
    {
        //Play animation first.
        if (_cardIsPlaced && _cardData.health <= 0 && !_cardData.isDead)
        {
            StartCoroutine(CardDeathRoutine());
        }
    }

    public void CardTakeDamage(Card attacker)
    {
        _cardData.health -= attacker._cardData.damage;
    }

    private IEnumerator CardDeathRoutine()
    {
        _cardData.isDead = true;
        yield return new WaitForSeconds(3f);
        _cardData.isDead = false;
        Destroy(gameObject);
    }
}
