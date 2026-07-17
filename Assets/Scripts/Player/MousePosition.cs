using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MousePosition : MonoBehaviour
{
    [SerializeField] private Camera _renderCamera;
    [SerializeField] private RawImage _rawImage;
    [SerializeField] private LayerMask _cardLayer;


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
        if (!TryGetHit(out RaycastHit hit) || InputManager.Instance.IsPointerOverUI) return;

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

        hit = default;

        if (!TryGetViewportPoint(out Vector2 viewportPoint)) return false;

        Ray ray = _renderCamera.ViewportPointToRay(viewportPoint);
        return Physics.Raycast(ray, out hit, Mathf.Infinity);

    }

    public Ray GetMouseRay()
    {
        if (!TryGetViewportPoint(out Vector2 viewportPoint))
            return new Ray(_renderCamera.transform.position, _renderCamera.transform.forward);
        return _renderCamera.ViewportPointToRay(viewportPoint);
    }

    private bool TryGetViewportPoint(out Vector2 viewportPoint)
    {
        viewportPoint = default;

        RectTransform rt = _rawImage.rectTransform;
        Vector2 screenPos = InputManager.Instance.MouseScreenPosition;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, screenPos, null, out Vector2 localPoint))
            return false;

        Rect r = rt.rect;
        float u = (localPoint.x - r.x) / r.width;
        float v = (localPoint.y - r.y) / r.height;

        if (u < 0 || u > 1 || v < 0 || v > 1) return false;

        viewportPoint = new Vector2(u, v);
        return true;
    }
}
