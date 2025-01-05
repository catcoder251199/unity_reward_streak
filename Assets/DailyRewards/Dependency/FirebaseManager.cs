using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;
    public bool IsRemoteConfigReady { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    public string GetStringValueRemoteConfig(string remoteKey, string fallbackValue)
    {
        return fallbackValue;
    }

    public int GetIntValueRemoteConfig(string remoteKey, int fallbackValue)
    {
        return fallbackValue;
    }
}
