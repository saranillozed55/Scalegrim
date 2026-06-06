using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MousePosition : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private LayerMask _cardLayer;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        InputManager.Instance.OnLeftClickPressed += HandleLeftClickPressed;
        InputManager.Instance.OnRightClickPressed += HandleRightClickPressed;
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnLeftClickPressed -= HandleLeftClickPressed;
            InputManager.Instance.OnRightClickPressed -= HandleRightClickPressed;
        }
    }

    private void HandleLeftClickPressed()
    {
        Debug.Log("Left Clicking");
        if (!TryGetHit(out RaycastHit hit)) return;

        var clickable = hit.collider.GetComponentInParent<IClickable>();
        if (clickable != null)
        {
            // Handle card click logic here
            clickable.OnClick();
        }
    }

    private void HandleRightClickPressed()
    {
        if (!TryGetHit(out RaycastHit hit)) return;
        Debug.Log("Clicked card: " + hit.collider.gameObject.name);
    }

    private bool TryGetHit(out RaycastHit hit)
    {
        Ray ray = _mainCamera.ScreenPointToRay(InputManager.Instance.MouseScreenPosition);
        return Physics.Raycast(ray, out hit, Mathf.Infinity);
    }
}
