using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using System.Threading;

public class Card : MonoBehaviour, IHoverable, IClickable
{
    private const int _placedCardLayer = 6;

    //[Header("Card DataSO")] // SO so we can change text mesh pro easier
    //[SerializeField] private CardData _cardData; // PROBLEM: If we have multiple of the same cards, the SO will be pointing to both of those so if they take damage they will both lose health

    [Header("Card Settings")]
    [SerializeField] private int baseDamage;
    [SerializeField] private int baseHealth;

    public PlainCardData _cardData { get; private set; }

    [Header("BroadCast Event Channels")]
    [SerializeField] private CardEventChannelSO _cardClicked;
    [SerializeField] private CameraStateEventChannel _cardSelected;

    private bool _cardIsSelected = false;
    public Vector3 _basePosition;
    public Vector3 _placedPosition;
    public Quaternion _baseRotation;
    public bool _cardIsPlaced = false;

    private bool _cardIsAttacking = false;


    private void Start()
    {
        _cardData = new PlainCardData(baseHealth, baseDamage);
    }

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


    public virtual async Awaitable PlayCardAttackAsync(Vector3 attackDirection, Card oppositeCard) // maybe dont use attackDirection but rather who owns this card
    {
        _cardIsAttacking = true;

        try
        {
            transform.DOKill();

            await transform.DOMove(_placedPosition + attackDirection * 0.3f, 0.15f).SetEase(Ease.OutQuad).AsyncWaitForCompletion(); // must have await because DOTween runs async in the background so must call await

            //Damage here as well
            ApplyDamage(oppositeCard);

            await transform.DOMove(_placedPosition, 0.15f).SetEase(Ease.InQuad).AsyncWaitForCompletion();
        }
        catch (Exception ex) 
        {
            Debug.LogWarning("Card action stopped because the card was removed: " + ex.Message);
            transform.DOKill();
        }
        finally
        {
            _cardIsAttacking = false;
        }
    }

    private void ApplyDamage(Card oppositeCard)
    {
        if (oppositeCard == null)
        {
            Debug.Log("This should do damage to the enemy/person");
            return;
        }
        else
        {
            oppositeCard._cardData._health -= this._cardData._damage;

            CardTakeDamage(oppositeCard);
            
            Debug.Log($"Opposite Card health: {oppositeCard._cardData._health} and Damage done to it: {_cardData._damage}");
        }
    }

    public async virtual void CheckCardDeath()
    {
        //Play animation first.
        if (_cardIsPlaced && _cardData._health <= 0 && !_cardData.isDead)
        {
            await CardDeathAsync();
        }
    }
    
    //opposite card takes damage
    public void CardTakeDamage(Card attacker)
    {
        CheckCardDeath();
        attacker._cardData._health -= this._cardData._damage;
    }

    private async Awaitable CardDeathAsync()
    {
        _cardData.isDead = true;
        try
        {
            await Awaitable.WaitForSecondsAsync(1f);
        }
        catch(Exception ex)
        {
            Debug.LogWarning("Card death action stopped because the card was removed: " + ex.Message);
        }
        finally
        {
            _cardData.isDead = false;
            Destroy(gameObject);
        }
    }
}
