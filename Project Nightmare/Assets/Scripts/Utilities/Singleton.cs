using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance { get; set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = (T) this;
        Initialise();
    }

    protected abstract void Initialise();

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}