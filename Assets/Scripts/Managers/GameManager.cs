using UnityEngine;

public class GameManager : GenericSingleton<GameManager>
{
    public bool onFocus = true;

    //private void Start()
    //{
    //    Cursor.lockState = CursorLockMode.Locked;
    //}
    private void Update()
    {
        Cursor.lockState = onFocus ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
