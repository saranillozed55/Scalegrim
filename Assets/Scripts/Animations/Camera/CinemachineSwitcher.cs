using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CinemachineSwitcher : MonoBehaviour
{
    [SerializeField] private InputAction _inputAction; // don't really use action because wan't this to be more even driven

    private Animator _animator;

    //update this for multiple cameras
    private bool _fpCamera = true;

    private int FpCamera = Animator.StringToHash("FPCamera");
    private int PlaceCardCamera = Animator.StringToHash("PlaceCardCamera");
    private int CardCamera = Animator.StringToHash("CardCamera");
    private int DeckCamera = Animator.StringToHash("DeckCamera");

    [Header("Listener to Event Channels")]
    [SerializeField] private CardEventChannelSO _cardClicked;
    [SerializeField] private CHSEventChannelSO _cardUnselected;
    [SerializeField] private CHSEventChannelSO _cardPlayed;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _cardClicked.onEventRaised += SwitchState;
        _cardPlayed.onEventRaised += SwitchCameraState;
        _cardUnselected.onEventRaised += SwitchCameraState;
        _inputAction.Enable();
    }
    private void OnDisable()
    {
        _cardClicked.onEventRaised -= SwitchState;
        _cardUnselected.onEventRaised -= SwitchCameraState;
        _cardPlayed.onEventRaised -= SwitchCameraState;
        _inputAction.Disable();
    }
    private void Start()
    {
        _inputAction.performed += _ => SwitchState(null); // just for testing
    }

    private void SwitchCameraState(HandState state)
    {
        if(state == HandState.InHand)
        {
            _animator.Play(FpCamera);
        }
    }

    private void SwitchState(Card card)
    { 
        if (card != null)
        {
            _animator.Play(PlaceCardCamera);
        }
        else
        {
            _animator.Play(FpCamera);
        }
        
        _fpCamera = !_fpCamera;
    }
}
