using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Skylink.GameFramework;
using UnityEngine;

namespace RewardStreak
{
    [CreateAssetMenu(fileName = "new RewardStreakConfig", menuName = "ScriptableObjects/RewardStreak/RewardStreakConfig")]
    public class RewardStreakConfigSO : ScriptableObject
    {
        [SerializeField] private string dataKey = "RewardStreak";
        [SerializeField] private bool enableLoadConfig = true;
        [SerializeField] private int rewardStreakMinLevel = RewardStreakConstants.RewardStreakMinLevelDefault;
        [SerializeField] private RewardStreakConfigData configData;
        
#if UNITY_EDITOR
        [SerializeField, Header("Inspector-Only Fields")] private string jsonConfig;
#endif

        public int MinLevelRewardStreak => rewardStreakMinLevel;
        public List<Reward> FreeRewards => configData.FreeRewards;
        public List<Reward> AdsRewards => configData.AdsRewards;
        public int FreeRewardCooldown => configData.FreeRewardCooldown;
        public int AdsRewardCooldown => configData.AdsRewardCooldown;

        private static FirebaseManager GetFirebaseManager() => FirebaseManager.Instance;

        [ContextMenu("Load config from remote")]
        public void LoadConfigFromRemote()
        {
            if (!enableLoadConfig)
                return;

            var loadedData = LoadConfigFromRemoteConfig();
            if (loadedData != null)
                configData = loadedData;

            var firebaseManager = GetFirebaseManager();
            if (firebaseManager != null)
                rewardStreakMinLevel = firebaseManager.GetIntValueRemoteConfig(RewardStreakConstants.RewardStreakMinLevelKey,
                    DailyRewardsConstants.MinLevelToReceiveRewards);
        }

        [ContextMenu("Load config from local")]
        public async void LoadConfigFromLocal()
        {
            if (!enableLoadConfig)
                return;
            configData = await LoadConfigFromJsonFile();
        }

        private RewardStreakConfigData LoadConfigFromRemoteConfig()
        {
            var fileName = dataKey;
            var remoteConfig = JsonHelper.LoadRemoteConfig(fileName);
            
            return !string.IsNullOrEmpty(remoteConfig) ? ConvertStringToData(remoteConfig) : null;
        }

        private async UniTask<RewardStreakConfigData> LoadConfigFromJsonFile()
        {
            var fileName = dataKey + ".dat";
            var localConfig = await JsonHelper.LoadLocalConfig(fileName);
            
            return !string.IsNullOrEmpty(localConfig) ? ConvertStringToData(localConfig) : null;
        }

        private static RewardStreakConfigData ConvertStringToData(string stringConfig)
        {
            var ret = JsonUtility.FromJson(stringConfig, typeof(RewardStreakConfigData)) as RewardStreakConfigData;
            return ret;
        }
        
#if UNITY_EDITOR
        [ContextMenu("Get Json from config")]
        public void GetJsonFromData()
        {
            if (configData == null)
                return;
            jsonConfig = configData.ConvertDataToString();
            Debug.Log(jsonConfig);
        }
        
        [ContextMenu("Convert Json to Data")]
        public void ConvertJsonToData()
        {
            if (!string.IsNullOrEmpty(jsonConfig))
                configData = ConvertStringToData(jsonConfig);
        }
#endif
    }
}