using DG.Tweening;
using UnityEngine;
public enum Owner {
    Player,
    Enemy,
}

public class CardDropArea : MonoBehaviour, ICardDropArea, IClickable, IHoverable
{
    [field: SerializeField] public Owner SlotOwner { get; private set; } // this is set in the insepctor

    [Range(0,3)]
    [SerializeField] private int laneIndex; // Define 0 through 3 inside the unity inspector


    public bool IsFull()
    {
        if(SlotOwner == Owner.Player)
        {
            return BoardLaneManager.Instance.LogicLanes[laneIndex].IsPlayerSideOccupied;
        }
        else
        {
            return BoardLaneManager.Instance.LogicLanes[laneIndex].IsEnemySideOccupied;
        }
    }

    public void OnCardDrop(CardView playedCard)
    {
        if(playedCard != null)
        {
            BoardLaneManager.Instance.PlaceCardInLane(playedCard.CardModel, laneIndex, SlotOwner);
            playedCard.SetBasePosition(transform.position);
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
        if (SelectionManager.Instance.SelectedHandCard != null && !IsFull() && SlotOwner == Owner.Player)
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
