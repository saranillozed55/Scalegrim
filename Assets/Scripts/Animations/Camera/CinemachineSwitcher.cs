using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public enum CameraState
{
    FPCamera,
    BoardCamera,
    PlayerHandCamera,
    PlayerDeckCamera,
}

public class CinemachineSwitcher : GenericSingleton<CinemachineSwitcher>
{
    private CameraState _currentCameraState;

    [Header("References")]
    [Space]
    [SerializeField] private CinemachineCamera[] _cameras;

    [SerializeField] private CinemachineCamera _firstPersonCamera;
    [SerializeField] private CinemachineCamera _boardCamera;
    [SerializeField] private CinemachineCamera _startingCamera;
    [SerializeField] private CinemachineCamera _playerDeckCamera;
    [SerializeField] private CinemachineCamera _playerHandCamera;

    private Dictionary<CameraState, CinemachineCamera> _cameraMap;

    private CinemachineCamera _currentCamera;


    private void OnEnable()
    {
        InputManager.Instance.OnBackButtonPressed += HandleBackButton;
        InputManager.Instance.OnForwardButtonPressed += HandleForwardButton;

    }
    private void OnDisable()
    {
        InputManager.Instance.OnBackButtonPressed -= HandleBackButton;
        InputManager.Instance.OnForwardButtonPressed -= HandleForwardButton;
    }

    private void Update()
    {

    }

    private void Start()
    {
        InitCameras();
    }

    private void InitCameras()
    {
        _currentCamera = _startingCamera;
        _cameraMap = new Dictionary<CameraState, CinemachineCamera>
        {
            {CameraState.FPCamera, _firstPersonCamera},
            {CameraState.BoardCamera, _boardCamera},
            {CameraState.PlayerDeckCamera, _playerDeckCamera},
            {CameraState.PlayerHandCamera, _playerHandCamera}
        };
    }

    private void HandleForwardButton()
    {
        if (_currentCameraState == CameraState.PlayerHandCamera)
        {
            SwitchState(CameraState.FPCamera);
        }
        else if (_currentCameraState == CameraState.FPCamera)
        {
            SwitchState(CameraState.BoardCamera);
        }
    }

    private void HandleBackButton()
    {
        if (_currentCameraState == CameraState.FPCamera)
        {
            SwitchState(CameraState.PlayerHandCamera);
        }
        else if (_currentCameraState == CameraState.BoardCamera)
        {
            SwitchState(CameraState.FPCamera);
        }
    }

    public void SwitchState(CameraState newState)
    {
        if (!_cameraMap.TryGetValue(newState, out CinemachineCamera targetCamera))
        {
            Debug.LogWarning($"[CinemachineManager/CinemachineSwitcher] No map registered for state: {newState}");
            return;
        }

        if (_currentCamera == targetCamera) return;
        _currentCamera.Priority = 0;
        _currentCamera = targetCamera;
        _currentCameraState = newState;
        _currentCamera.Priority = 1;
    }

    public void FocusBoardView()
    {
        SwitchState(CameraState.BoardCamera);
    }
    public void FocusFPCameraView()
    {
        SwitchState(CameraState.FPCamera);
    }
}
