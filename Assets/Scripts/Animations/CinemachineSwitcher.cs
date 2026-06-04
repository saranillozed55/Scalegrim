using UnityEngine;
using UnityEngine.InputSystem;

public class CinemachineSwitcher : MonoBehaviour
{
    [SerializeField] private InputAction action; // don't really use action because wan't this to be more even driven

    private Animator _animator;

    //update this for multiple cameras
    private bool _fpCamera = true;

    private int FpCamera = Animator.StringToHash("FPCamera");
    private int BoardCamera = Animator.StringToHash("BoardCamera");
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        action.Enable();
    }
    private void OnDisable()
    {
        action.Disable();
    }
    private void Start()
    {
        action.performed += _ => SwitchState();
    }

    private void SwitchState()
    {
        if(_fpCamera)
        {
            _animator.Play(FpCamera);
        }
        else
        {
            _animator.Play(BoardCamera);
        }
        _fpCamera = !_fpCamera;
    }
}
