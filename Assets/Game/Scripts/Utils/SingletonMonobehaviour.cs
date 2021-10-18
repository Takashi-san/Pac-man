using System;
using UnityEngine;

public class SingletonMonobehaviour<T> : MonoBehaviour where T : SingletonMonobehaviour<T> {
    public static T Instance;
    bool _isInstance = false;
    
    protected virtual void Awake() {
        if (Instance != null) {
            Debug.LogWarning($"[Singleton] Multiple singleton of type: {typeof(T).FullName}");
            Destroy(this);
            return;
        }

        Instance = this as T;
        _isInstance = true;
    }

    protected bool IsSingletonInstance() {
        return _isInstance;
    }
}