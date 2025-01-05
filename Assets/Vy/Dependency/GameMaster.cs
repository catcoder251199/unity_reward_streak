using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    [SerializeField] private UserProfile userProfile;
    public bool simulateWatchAdSuccess = true;
    public static GameMaster Instance { get; private set; }
    public UserProfile GetUserProfile() => userProfile;
    public int DisplayLevel => userProfile.CurrentLevel;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    public async UniTaskVoid ShowRewardedVideo(Action onWatchAdSuccess, Action onWatchAdFailed, string adPlacement)
    {
        if (simulateWatchAdSuccess)
        {
            Debug.Log("init ad...");
            await UniTask.Delay(TimeSpan.FromSeconds(0.25));
            Debug.Log("user watching ad...");
            await UniTask.Delay(TimeSpan.FromSeconds(1.25));
            Debug.Log("user watch ad success");
            onWatchAdSuccess();
        }
        else
        {
            Debug.Log("init ad...");
            await UniTask.Delay(TimeSpan.FromSeconds(0.5));
            Debug.Log("user watch ad failed");
        }
    }
}
