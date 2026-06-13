using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum CameraState { 
    FPCamera,
    BoardCamera,
    HandCamera,
    PlayerDeckCamera,
}

public class CinemachineSwitcher : MonoBehaviour
{
    private CameraState _cameraState;

    [Header("Listener to Event Channels")]
    [SerializeField] private CameraStateEventChannel _cardSelectedCam;
    [SerializeField] private CameraStateEventChannel _cardUnselectedCam;
    [SerializeField] private CameraStateEventChannel _onEndTurn;

    [Header("References")]
    [Space]
    [SerializeField] private CinemachineCamera[] _cameras;
        
    [SerializeField] private CinemachineCamera _firstPersonCamera;
    [SerializeField] private CinemachineCamera _boardCamera;
    [SerializeField] private CinemachineCamera _startingCamera;

    private CinemachineCamera _currentCamera;

    private void OnEnable()
    {

        _cardSelectedCam.onEventRaised += SwitchCameraState;
        _cardUnselectedCam.onEventRaised += SwitchCameraState;
        _onEndTurn.onEventRaised += SwitchCameraState;
    }
    private void OnDisable()
    {
        _cardSelectedCam.onEventRaised -= SwitchCameraState;
        _cardUnselectedCam.onEventRaised -= SwitchCameraState;
        _onEndTurn.onEventRaised += SwitchCameraState;
    }
    private void Start()
    {
        InitializeStartingCamera();        
    }
    private void InitializeStartingCamera()
    {
        _currentCamera = _startingCamera;
        for(int i = 0; i < _cameras.Length; i++)
        {
            if (_cameras[i] == _currentCamera)
            {
                _cameras[i].Priority = 20;
            }
            else
            {
                _cameras[i].Priority = 10;
            }
        }
    }

    private void SwitchCameraState(CameraState state)
    {
        switch(state) {
            case CameraState.FPCamera:
                SwitchCamera(_firstPersonCamera);
                break;
            case CameraState.BoardCamera:
                SwitchCamera(_boardCamera);
                break;

        }
    }

    private void SwitchCamera(CinemachineCamera newCamera)
    {
        _currentCamera = newCamera;
        _currentCamera.Priority = 20;
        for(int i = 0; i < _cameras.Length;i++)
        {
            if (_cameras[i] != newCamera)
            {
                _cameras[i].Priority = 10;
            }
        }
    }

}
