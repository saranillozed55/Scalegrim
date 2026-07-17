using UnityEngine;

public class SelectionManager : GenericSingleton<SelectionManager>
{

    public Card SelectedHandCard { get; private set; }

    public void OnCardClicked(Card card)
    {
        //ignore if we don't want to click on cards while not player turn
        if(TurnManager.Instance.CurrentTurnState != TurnState.PlayerTurn)
        {
            return;
        }

        //clicking same card deselects it
        if(card == SelectedHandCard)
        {
            DeselectCard();
            return;
        }
        SelectCard(card);
    }

    private void SelectCard(Card card)
    {
        //Deselct previous card
        if (SelectedHandCard != null)
        {
            //deselect current card
            SelectedHandCard.Deselect();
        }

        SelectedHandCard = card;

        HandManager.Instance.CardTempLeave(card);

        card.Select();

        CinemachineSwitcher.Instance.FocusBoardView();
        //change camera
    }

    public void DeselectCard()
    {
        if (SelectedHandCard == null) return;

        HandManager.Instance.CardBackToHand(SelectedHandCard);

        SelectedHandCard.Deselect();

        SelectedHandCard = null;

        CinemachineSwitcher.Instance.FocusFPCameraView();
        //change camera
    }
    public void CardPlayedDeselect()
    {
        SelectedHandCard.Deselect();
        SelectedHandCard = null;
        CinemachineSwitcher.Instance.FocusFPCameraView();
    }
}
