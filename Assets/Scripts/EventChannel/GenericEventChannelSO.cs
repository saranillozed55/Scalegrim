using System;
using UnityEngine;

public abstract class GenericEventChannelSO<T> : ScriptableObject
{
    public event Action<T> onEventRaised;

    public void RaiseEvent(T paramer)
    {
        onEventRaised?.Invoke(paramer);
    }
}
