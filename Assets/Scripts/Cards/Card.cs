using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using System.Threading;

public class Card : MonoBehaviour, IHoverable, IClickable
{
    private const int _placedCardLayer = 6;

    [Header("Card Settings")]
    [SerializeField] private int baseDamage;
    [SerializeField] private int baseHealth;
    [SerializeField] private int baseCost;

    public PlainCardData _cardData { get; private set; }

    private bool _cardIsSelected = false;
    public Vector3 _basePosition;
    public Vector3 _placedPosition;
    public Quaternion _baseRotation;
    public bool _cardIsPlaced = false;

    private bool _hoverable = false;

    //Audio
    [Header("Audio")]
    [Space]
    [SerializeField] private AudioClip _audioClip;
    private AudioSource _audioSource;
    private CardAudio _cardAudio;

    //public bool IsInteractable => !_cardIsPlaced // add this

    public int BaseDamage => baseDamage;
    public int BaseHealth => baseHealth;
    public int HealthCurrent
    {
        get
        {
            return _cardData._health;
        }
        private set
        {
            _cardData._health = Math.Max(0,value); // health can never drop below 0
        }
    }
    public int DamageCurrent
    {
        get
        {
            return _cardData._attackDamage;
        }
        private set
        {
            _cardData._attackDamage = value;
        }
    }
    public bool IsCardDead
    {
        get
        {
            return _cardData.isDead;
        }
        set
        {
            _cardData.isDead = value;
        }
    }

    public void SetHoverable(bool value) => _hoverable = value;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _cardData = new PlainCardData(baseHealth, baseDamage, baseCost);

        _cardAudio = new CardAudio(_audioSource, _audioClip, transform);
    }

    public void CardIsPlayed()
    {
        _cardIsPlaced = true;
        gameObject.layer = _placedCardLayer;
        transform.DOKill();
        transform.DORotateQuaternion(CardRotations._cardFaceFlatUp, 0.02f);
        foreach(Transform child in transform)
        {
            child.gameObject.layer = _placedCardLayer;
        }
    }

    public virtual void OnHoverEnter()
    {
        if (_cardIsPlaced || !_hoverable) return;
        transform.DOKill(); // Stop any ongoing tweens to prevent conflicts
        transform.DOMove(_basePosition + Vector3.up * 0.1f + Vector3.back * 0.02f, 0.2f);
        transform.DORotateQuaternion(_baseRotation, 0.25f);

        _cardAudio.PlayHoverSound();
    }
    public virtual void OnHoverExit()
    {
        if (_cardIsPlaced || !_hoverable) return;
        transform.DOKill(); // Stop any ongoing tweens to prevent conflicts
        transform.DOMove(_basePosition, 0.25f);
        transform.DORotateQuaternion(_baseRotation, 0.25f);
    }

    public virtual void OnClick()
    {
        if (_cardIsSelected || _cardIsPlaced || !_hoverable) return;

        SelectionManager.Instance.OnCardClicked(this);
    }

    //<REFACTOR> -- Move this to somewhere else because we want the damage to be done on a different scripts. More cleaner
    //This script is doing to much to the other card, and should be done by a different script

    public virtual async Awaitable PlayCardAttackAsync(Vector3 attackDirection, Card oppositeCard) // maybe dont use attackDirection but rather who owns this card
    {
        //_cardIsAttacking = true;
        try
        {
            transform.DOKill();

            await transform.DOMove(_placedPosition + attackDirection * 0.3f, 0.15f).SetEase(Ease.OutQuad).AsyncWaitForCompletion(); // must have await because DOTween runs async in the background so must call await

            //Damage here as well - may want to change this damaging system to use manager instead
            //ApplyDamage(oppositeCard);

            CardsDamager cardDamager = new(this, oppositeCard);
            cardDamager.ApplyDamage();

            await transform.DOMove(_placedPosition, 0.15f).SetEase(Ease.InQuad).AsyncWaitForCompletion();
        }
        catch (Exception ex) 
        {
            Debug.LogWarning("Card action stopped because the card was removed: " + ex.Message);
            transform.DOKill();
        }
        //finally
        //{
        //    _cardIsAttacking = false;
        //}
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

    public void Select()
    {
        Debug.Log("Card was selected");
        //HandManager.Instance.CardTempLeave(this);
    }
    public void Deselect()
    {
        Debug.Log("Card was deselected");
        //HandManager.Instance.CardBackToHand();
    }
}
