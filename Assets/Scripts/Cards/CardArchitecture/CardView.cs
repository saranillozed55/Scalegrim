using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class CardView : MonoBehaviour, IClickable, IHoverable
{

    private static readonly Dictionary<CardModel, CardView> _lookup = new();
    public static CardView GetView(CardModel model) =>
        model != null && _lookup.TryGetValue(model, out var view) ? view : null;


    [Header("View References")]
    //[SerializeField] private SpriteRenderer _cardImage;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _cost;
    [SerializeField] private TMP_Text _health;
    [SerializeField] private TMP_Text _attack;

    private CardModel card;
    public CardModel CardModel => card;

    [Header("Player Card Properties")]
    private Vector3 _basePosition;
    private Quaternion _baseRotation;
    private Vector3 _placedPosition;

    public void SetBasePosition(Vector3 value) => _basePosition = value;
    public void SetBaseRotation(Quaternion value) => _baseRotation = value; // move this to a function that does the _basePosition and _baseRotation, as well as SetBasePosition
    public void SetPlacedPosition(Vector3 value) => _placedPosition = value;

    private Tween _moveTween;
    private Tween _rotateTween;
    private Tween _hoverTween;

    public bool IsAnimating { get; private set; }
    public Quaternion BaseRotation { get; private set; }

    public void InitCard(CardModel card)
    {
        this.card = card;
        _lookup[card] = this;
        _name.text = card.Name;
        _cost.text = card.Cost.ToString();
        _health.text = card.Health.ToString();
        _attack.text = card.AttackDamage.ToString();
    }

    public virtual void OnHoverEnter()
    {
        if (card.CardPlaced || !card.CardHoverable || IsAnimating) return;

        _hoverTween?.Kill();

        SoundFXManager.Instance.Play(card.cardData, CardAudioType.Hover);
        _hoverTween = transform.DOMove(_basePosition + Vector3.up * 0.1f + Vector3.back * 0.02f, 0.2f);
    }
    public virtual void OnHoverExit()
    {
        if (card.CardPlaced || !card.CardHoverable || IsAnimating) return;
        _hoverTween?.Kill();

        _hoverTween = transform.DOMove(_basePosition, 0.2f);
    }
    public virtual void OnClick()
    {
        if (card.CardSelected || card.CardPlaced || !card.CardHoverable) return;

        SelectionManager.Instance.OnCardClicked(this);
    }

    public virtual async Awaitable CardAttackAsync(CardModel attackingCard, CardModel defendingCard)
    {
        AreaOwnerType? owner = attackingCard.BoardOwner;
        try
        {
            if (owner == null) return;

            transform.DOKill();

            Vector3 direction = owner == AreaOwnerType.PlayerActive ? Vector3.forward : Vector3.back;

            await transform.DOMove(_placedPosition + direction * 0.3f, 0.15f).SetEase(Ease.OutQuad).AsyncWaitForCompletion();

            card.CardAttack(attackingCard, defendingCard);
            //refresh display

            await transform.DOMove(_placedPosition, 0.15f).SetEase(Ease.InQuad).AsyncWaitForCompletion();

        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            transform.DOKill();
        }
    }
    public void Select()
    {
        //Debug.Log("Card was selected");
    }
    public void Deselect()
    {
        //Debug.Log("Card was deselected");
    }

    public async Task MoveCardToPosition(Vector3 targetPosition)
    {
        IsAnimating = true;
        _moveTween?.Kill();
        _rotateTween?.Kill();

        Sequence sequence = DOTween.Sequence();
        sequence.Join(transform.DOMove(targetPosition, 0.3f)); //magic number

        await sequence.AsyncWaitForCompletion();

        IsAnimating = false;
    }

    public async Task MoveCardToPosition(Vector3 targetPosition, Quaternion targetRotation) // ADD DURATION TO THIS FLOAT
    {
        IsAnimating = true;
        _moveTween?.Kill();
        _rotateTween?.Kill();

        Sequence sequence = DOTween.Sequence();

        _moveTween = transform.DOMove(targetPosition, 0.3f);
        _rotateTween = transform.DORotateQuaternion(targetRotation, 0.3f);

        sequence.Join(_moveTween);
        sequence.Join(_rotateTween);

        await sequence.AsyncWaitForCompletion();

        IsAnimating = false;
    }
    public async Task MoveCardToPositionWithDelay(Vector3 targetPosition, Quaternion targetRotation, float delay)
    {
        IsAnimating = true;
        _moveTween?.Kill();
        _rotateTween?.Kill();

        Sequence sequence = DOTween.Sequence();

        _moveTween = transform.DOMove(targetPosition, 0.3f);
        _rotateTween = transform.DORotateQuaternion(targetRotation, 0.3f);

        sequence.Join(_moveTween);
        sequence.Join(_rotateTween);
        sequence.SetDelay(delay);

        await sequence.AsyncWaitForCompletion();
    }

    public async Task RotateCard(Quaternion targetRotation)
    {
        IsAnimating = true;
        _rotateTween?.Kill();

        Sequence sequence = DOTween.Sequence();
        sequence.Join(transform.DORotateQuaternion(targetRotation, 0.3f));

        await sequence.AsyncWaitForCompletion();
        IsAnimating = false;
    }

    public void CardDeath()
    {
        //then should have animation as well
        GameObject.Destroy(this.gameObject);
    }
    public void CardTakeDamage()
    {

    }
}

