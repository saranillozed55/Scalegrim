using System;
using UnityEngine;

public class SelectionManager : GenericSingleton<SelectionManager>
{

    public CardView SelectedHandCard { get; private set; }

    public void OnCardClicked(CardView card)
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

    private void SelectCard(CardView card)
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

    public async void DeselectCard()
    {
        if (SelectedHandCard == null) return;


        SelectedHandCard.Deselect();

        SelectedHandCard = null;

        CinemachineSwitcher.Instance.FocusFPCameraView();
        //change camera
        try
        {
            await HandManager.Instance.CardBackToHand(SelectedHandCard);
        }
        catch(Exception e)
        {
            Debug.LogError("Error[Selection Manager]: " + e.Message);
        }
    }
    public void CardPlayedDeselect()
    {
        SelectedHandCard.Deselect();
        SelectedHandCard = null;
        CinemachineSwitcher.Instance.FocusFPCameraView();
    }
}
