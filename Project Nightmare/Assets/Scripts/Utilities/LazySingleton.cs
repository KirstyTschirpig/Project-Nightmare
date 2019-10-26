using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LazySingleton<T> : MonoBehaviour where T : LazySingleton<T>
{

    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject($"LS_{typeof(T).Name}").AddComponent<T>();
            }

            return instance;
        }
    }

    void Awake()
    {
        Initialise();
    }

    protected abstract void Initialise();

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }

}
