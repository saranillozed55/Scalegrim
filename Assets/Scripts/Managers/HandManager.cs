using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class HandManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxHandSize;
    [SerializeField] private float cardSpacing = 0.08f; // Adjust this value to increase/decrease spacing between cards
    [SerializeField] private LayerMask _cardLayer;
    [SerializeField] private GameObject cardPrefab; // change this to specific card prefabs

    [Header("References")]
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform spawnPoint;
    //[SerializeField] private float cameraFacingBlend = 0.5f;

    private GameObject _currentHoveredCard;

    private List<GameObject> handCards = new();

    private void Update()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            DrawCard();
        }
        if(Keyboard.current.dKey.wasPressedThisFrame)
        {
            ClearCards();
        }
        HandleCardHover();
    }

    private void DrawCard()
    {
        if(handCards.Count >= maxHandSize) return;
        GameObject newCard = Instantiate(cardPrefab, spawnPoint.position, spawnPoint.rotation);
        handCards.Add(newCard);
        UpdateCardPosition();
    }

    private void UpdateCardPosition()
    {
        if (handCards.Count == 0) return;

        float totalSpread = (handCards.Count - 1) * cardSpacing;
        //float cardSpacing = 1f / handCards.Count;
        float firstCardPosition = 0.5f - totalSpread / 2f; // Center the cards around the middle of the spline
        Spline spline = splineContainer.Spline;

        for (int i = 0; i < handCards.Count; i++)
        {
            float p = firstCardPosition + i * cardSpacing;

            // Transform from spline local space → world space
            Vector3 splinePosition = splineContainer.transform.TransformPoint(spline.EvaluatePosition(p));

            // Tangent is forward, up is up — construct rotation correctly
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion splineRotation = Quaternion.LookRotation(forward, up);

            //// Rotation that points the card toward the camera
            //Vector3 dirToCamera = Camera.main.transform.position - splinePosition;
            //Quaternion cameraRotation = Quaternion.LookRotation(-dirToCamera, up);

            //// Blend between the two
            //Quaternion rotation = Quaternion.Slerp(splineRotation, cameraRotation, cameraFacingBlend);

            splinePosition += up * (i * 0.02f); // Add vertical offset based on card index

            handCards[i].GetComponentInChildren<Card>().SetBasePosition(splinePosition); // Set the base position for hover effects
            handCards[i].transform.DOMove(splinePosition, 0.25f);

            // Fix 3: Use world-space rotation tween
            handCards[i].transform.DORotateQuaternion(splineRotation, 0.25f);
        }
    }

    private void HandleCardHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.MouseScreenPosition);
        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _cardLayer))
        {
            GameObject hitCard = hit.collider.transform.root.gameObject; // handCards contains the root gameObject of the card prefab, so we need to get the root of the hit collider

            if (hitCard == _currentHoveredCard)
            {
                return; // Already hovering this card, do nothing
            }
            if(_currentHoveredCard != null)
            {
                _currentHoveredCard.GetComponentInChildren<IHoverable>()?.OnHoverExit();
            }
            //Enter new, only if it's a card in hand
            if(handCards.Contains(hitCard))
            {
                _currentHoveredCard = hitCard;
                _currentHoveredCard.GetComponentInChildren<IHoverable>()?.OnHoverEnter();
            }
            else
            {
                _currentHoveredCard = null;
            }
        }
        else
        {
            //ray cast hit nothing
            if(_currentHoveredCard != null)
            {
                _currentHoveredCard.GetComponentInChildren<IHoverable>()?.OnHoverExit();
                _currentHoveredCard = null;
            }
        }
    }

    public void ClearCards()
    {
        if (handCards == null || handCards.Count == 0) return;
        foreach(var card in handCards)
        {
            Destroy(card);
        }
        handCards.Clear();
    }
}
