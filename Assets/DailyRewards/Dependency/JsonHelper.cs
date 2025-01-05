using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Skylink.GameFramework;
using Cysharp.Threading.Tasks;
using System;
public class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
    public static async UniTask<T> ConvertJsonToConfig<T>(string remoteKey) where T : class
    {
        string fileName = remoteKey + ".dat";
        string localConfig = await LoadLocalConfig(fileName);
        string remoteConfig = LoadRemoteConfig(remoteKey);
        if (string.IsNullOrEmpty(remoteConfig))
        {
            if (!string.IsNullOrEmpty(localConfig))
                return JsonUtility.FromJson<T>(localConfig);
            return null;
        }
        else
        {
            return JsonUtility.FromJson<T>(remoteConfig);
        }
    }
    public static async UniTask<T[]> ConvertJsonToConfigs<T>(string remoteKey)
    {
        string fileName = remoteKey + ".dat";
        string localConfig = await LoadLocalConfig(fileName);
        string remoteConfig = LoadRemoteConfig(remoteKey);
        if (string.IsNullOrEmpty(remoteConfig))
        {
            if (!string.IsNullOrEmpty(localConfig))
                return FromJson<T>(localConfig);
            return null;
        }
        else
        {
            return FromJson<T>(remoteConfig);
        }
    }
    public static async UniTask<string> LoadLocalConfig(string fileName)
    {
        string localConfig = string.Empty;
        bool isLoaded = false;
#if UNITY_ANDROID && !UNITY_EDITOR
        await CommonUtils.LoadTextFileFromStreamingAsset(fileName, (isSuccess, content) =>
        {
            if (isSuccess)
            {
                localConfig = content;
                // Debug.Log($"[{remoteKey}]Get booster config from file successfully: ");
            }
            else
            {
                // Debug.LogError($"[{remoteKey}]" + content);
            }
            isLoaded = true;
        });
#else
        localConfig = CommonUtils.LoadTextFileFromStreamingAsset(fileName);
        isLoaded = true;
#endif
        while (!isLoaded) await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        return localConfig;
    }
    public static string LoadRemoteConfig(string remoteKey)
    {
        string remoteConfig = string.Empty;
        var firebaseManager = FirebaseManager.Instance;
        if (firebaseManager != null && firebaseManager.IsRemoteConfigReady)
            remoteConfig = FirebaseManager.Instance.GetStringValueRemoteConfig(remoteKey, string.Empty);
        return remoteConfig;
    }
}
