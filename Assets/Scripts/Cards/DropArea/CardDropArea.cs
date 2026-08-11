using DG.Tweening;
using UnityEngine;
public enum AreaOwnerType {
    PlayerActive,
    EnemyActive,
    EnemyQueue,
}

public partial class CardDropArea : MonoBehaviour, ICardDropArea, IClickable, IHoverable
{
    private CardDropAreaData cardDropAreaData;
    [field: SerializeField] public AreaOwnerType SlotOwner { get; private set; } // this is set in the insepctor

    [Range(0,3)]
    [SerializeField] private int laneIndex; // Define 0 through 3 inside the unity inspector

    public void Init(CardDropAreaData data)
    {
        cardDropAreaData = data;

        //subscribe to event here

        Debug.Log($"Environment: {data.Environment}, SlotOwner: {SlotOwner} ");

        UpdateEnvironmentVisuals(cardDropAreaData.Environment);
    }

    public bool IsAreaTaken()
    {
        //return cardDropAreaData.IsTaken;

        if(SlotOwner == AreaOwnerType.PlayerActive)
        {
            return BoardLaneManager.Instance.LogicLanes[laneIndex].IsPlayerSideOccupied;
        }
        else
        {
            return BoardLaneManager.Instance.LogicLanes[laneIndex].IsEnemySideOccupied;
        }
    }

    private void HandleEnvironmentChanged()
    {
        
    }
    private void UpdateEnvironmentVisuals(EnvironmentType environment)
    {

    }

    public void OnCardDrop(CardView playedCard)
    {
        if(playedCard != null)
        {
            BoardLaneManager.Instance.PlaceCardInLane(playedCard.CardModel, laneIndex, SlotOwner);
            playedCard.SetPlacedPosition(transform.position);
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
        if (SelectionManager.Instance.SelectedHandCard != null && !IsAreaTaken() && SlotOwner == AreaOwnerType.PlayerActive)
        {
           _ = HandManager.Instance.PlayCurrentCard(this);
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
