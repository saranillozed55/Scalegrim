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

    private void OnEnable()
    {
        InputManager.Instance.OnLeftClickPressed += HandleLeftClickPressed;
        InputManager.Instance.OnRightClickPressed += HandleRightClickPressed;
    }
    private void OnDisable()
    {
        InputManager.Instance.OnLeftClickPressed -= HandleLeftClickPressed;
        InputManager.Instance.OnRightClickPressed -= HandleRightClickPressed;
    }

    private void HandleLeftClickPressed()
    {
        if (!TryGetHitCard(out RaycastHit hit)) return;
        Debug.Log("Clicked card: " + hit.collider.gameObject.name);
    }

    private void HandleRightClickPressed()
    {
        if (!TryGetHitCard(out RaycastHit hit)) return;
        Debug.Log("Clicked card: " + hit.collider.gameObject.name);
    }

    private void Update()
    {
        
    }

    private bool TryGetHitCard(out RaycastHit hit)
    {
        Ray ray = _mainCamera.ScreenPointToRay(InputManager.Instance.MouseScreenPosition);
        return Physics.Raycast(ray, out hit, Mathf.Infinity, _cardLayer);
    }
}
