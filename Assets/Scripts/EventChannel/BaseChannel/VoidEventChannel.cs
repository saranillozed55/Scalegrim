using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Event/Void Event Channel")]
public class VoidEventChannel : ScriptableObject
{

    public event Action onEventRaised;

    public void RaiseEvent()
    {
        onEventRaised?.Invoke();
    }
}
