using UnityEngine;
public enum Faction {
    Player,
    Enemy,
}

public class CardDropArea : MonoBehaviour, ICardDropArea, IClickable
{
    [field: SerializeField]
    public Faction SlotFaction { get; private set; }

    private bool _isFull = false;

    public void OnCardDrop()
    {
        // just play animation
        _isFull = true;
        PlaySlamAnimation();
    }

    public void OnClick()
    {
        if (HandManager.Instance.CurrentSelectedCard != null && HandManager.Instance.CurrentHandState == HandState.Selected && !_isFull)
        {
            HandManager.Instance.PlayCurrentCard(this);
        }
    }
    private void PlaySlamAnimation()
    {

    }
}
