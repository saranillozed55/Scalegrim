using DG.Tweening;
using UnityEngine;
public enum Owner {
    Player,
    Enemy,
}

public class CardDropArea : MonoBehaviour, ICardDropArea, IClickable, IHoverable
{
    [field: SerializeField] public Owner SlotOwner { get; private set; } // this is set in the insepctor

    [Range(0,2)]
    [SerializeField] private int laneIndex; // Define 0 through 2 inside the unity inspector


    private bool IsFull()
    {
        if(SlotOwner == Owner.Player)
        {
            return BoardLaneManager.Instance.logicLanes[laneIndex].IsPlayerSideOccupied;
        }
        else
        {
            return BoardLaneManager.Instance.logicLanes[laneIndex].IsEnemySideOccupied;
        }
    }

    public void OnCardDrop(Card playedCard)
    {
        Card card = playedCard;
        if(card != null)
        {
            BoardLaneManager.Instance.PlaceCardInLane(card, laneIndex, SlotOwner);
            card._placedPosition = transform.position;
        }
        else
        {
            Debug.LogWarning("Card drop area on Lane " + laneIndex + " recieved a null card");
        }
        PlaySlamAnimation();
    }

    public void LoadCardAreas()
    {

    }

    public void OnClick()
    {
        if (HandManager.Instance.CurrentSelectedCard != null && HandManager.Instance.CurrentHandState == HandState.Selected && !IsFull() && SlotOwner == Owner.Player)
        {
            HandManager.Instance.PlayCurrentCard(this);
        }
    }
    private void PlaySlamAnimation()
    {
        //Tiny particle animations probably
    }

    public void OnHoverEnter()
    {

    }
    public void OnHoverExit()
    {

    }
}
