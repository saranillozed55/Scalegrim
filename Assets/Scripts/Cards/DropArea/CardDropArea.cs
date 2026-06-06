using UnityEngine;

public class CardDropArea : MonoBehaviour, ICardDropArea, IClickable
{
    public void OnCardDrop()
    {
        // just play animation
    }

    public void OnClick()
    {
        if(HandManager.Instance.CurrentSelectedCard != null && HandManager.Instance.CurrentHandState == HandState.Selected)
        {
            HandManager.Instance.PlayCurrentCard(this);
        }
    }
}
