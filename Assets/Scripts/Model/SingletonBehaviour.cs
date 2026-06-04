using UnityEngine;

public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType(typeof(T)) as T;
                if (_instance == null)
                {
                    if (Application.isPlaying)
                    {
                        Debug.LogWarning(string.Format("There's no active {0} object", typeof(T)));
                    }
                }
            }

            return _instance;
        }
    }

    public static T it
    {
        get => Instance;
    }

    protected virtual void OnDestroy()
    {
        if (IsMyInstance())
        {
            _instance = null;
        }
    }

    protected bool IsMyInstance()
    {
        return _instance == this;
    }

    public static bool IsLive => _instance != null;
}