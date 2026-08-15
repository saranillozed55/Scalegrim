using UnityEngine;

public abstract class GenericSingleton<T> : MonoBehaviour where T : MonoBehaviour 
{
    //private static T instance;
    public static T Instance
    {
        get;
        private set;
    }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(this as T);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
