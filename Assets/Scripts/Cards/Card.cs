using DG.Tweening;
using UnityEngine;

public class Card : MonoBehaviour, IHoverable
{
    private Vector3 _basePosition;

    public void SetBasePosition(Vector3 position)
    {
        _basePosition = position;
    }
    public void OnHoverEnter()
    {
        transform.DOKill(); // Stop any ongoing tweens to prevent conflicts
        transform.DOMove(_basePosition + Vector3.up * 0.3f, 0.2f);
    }
    public void OnHoverExit()
    {
        transform.DOKill(); // Stop any ongoing tweens to prevent conflicts
        transform.DOMove(_basePosition, 0.2f);
    }
}
