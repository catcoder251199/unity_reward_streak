using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DailyRewards
{
    [CreateAssetMenu(fileName = "DailyRewardsConfig", menuName = "ScriptableObjects/DailyRewards/DailyRewardsConfig")]
    public class DailyRewardsConfig : ScriptableObject
    {
        [Serializable]
        private class RawDateData
        {
            public int Day;
            public string Value;
        }

        [SerializeField] private string dataKey = "DailyRewards";
        [SerializeField] private int minLevelToReceiveRewards = DailyRewardsConstants.MinLevelToReceiveRewards;
        [SerializeField] private bool enableLoadConfig = true;
        [SerializeField] private List<DateData> configData = new();

#if UNITY_EDITOR
        [SerializeField, Header("Inspector-Only Fields")] private string jsonConfig;
#endif

        public int TotalDays => configData.Count;
        public List<DateData> RewardList => configData;
        public int MinLevelToReceiveRewards => minLevelToReceiveRewards;

        private static FirebaseManager GetFirebaseManager() => FirebaseManager.Instance;

        [ContextMenu("Load config from remote")]
        public void LoadConfigFromRemote()
        {
            if (!enableLoadConfig)
                return;

            var loadedData = LoadConfigFromRemoteConfig();
            if (loadedData != null && loadedData.Count != 0)
                configData = loadedData;

            var firebaseManager = GetFirebaseManager();
            if (firebaseManager != null)
                minLevelToReceiveRewards = firebaseManager.GetIntValueRemoteConfig("DailyRewardsMinLevel",
                    DailyRewardsConstants.MinLevelToReceiveRewards);
        }

        [ContextMenu("Load config from local")]
        public async void LoadConfigFromLocal()
        {
            if (!enableLoadConfig)
                return;
            configData = await LoadConfigFromJsonFile();
        }

        private List<DateData> LoadConfigFromRemoteConfig()
        {
            var fileName = dataKey;
            var remoteConfig = JsonHelper.LoadRemoteConfig(fileName);
            if (!string.IsNullOrEmpty(remoteConfig))
                return ConvertStringToData(remoteConfig);
            return new List<DateData>();
        }

        private async UniTask<List<DateData>> LoadConfigFromJsonFile()
        {
            var fileName = dataKey + ".dat";
            var localConfig = await JsonHelper.LoadLocalConfig(fileName);
            if (!string.IsNullOrEmpty(localConfig))
                return ConvertStringToData(localConfig);
            return new List<DateData>();
        }

        private List<DateData> ConvertStringToData(string stringConfig)
        {
            var logPrefix = $"{nameof(ConvertStringToData)}";
            List<DateData> ret = new();
            var rawConfig = JsonHelper.FromJson<RawDateData>(stringConfig);
            foreach (var t in rawConfig)
            {
                try
                {
                    var rewards = t.Value.Split('_');
                    var rewardList = new List<RewardItemData>();
                    foreach (var rewardStr in rewards)
                    {
                        var rewardInfo = rewardStr.Split("|");
                        if (rewardInfo.Length < 2 || string.IsNullOrEmpty(rewardInfo[0]) ||
                            !int.TryParse(rewardInfo[1], out var rewardAmount))
                            continue;
                        rewardList.Add(new RewardItemData(rewardInfo[0], rewardAmount));
                    }

                    if (rewardList.Count != 0)
                        ret.Add(new DateData(t.Day, rewardList));
                }
                catch (Exception exception)
                {
                    Debug.Log($"{logPrefix} Exception: {exception.Message}");
                }
            }

            return ret;
        }

        [ContextMenu("Get Json from config")]
        public void GetJsonFromData()
        {
            jsonConfig = ConvertDataToString(configData);
            Debug.Log(jsonConfig);
        }
        
        [ContextMenu("Convert Json to Data")]
        public void ConvertJsonToData()
        {
            if (!string.IsNullOrEmpty(jsonConfig))
                configData = ConvertStringToData(jsonConfig);
        }

        private static string ConvertDataToString(List<DateData> dataList)
        {
            if (dataList == null || dataList.Count == 0)
                return string.Empty;
            var strBuilder = new StringBuilder();
            strBuilder.Append("{\"Items\":[");

            var dayFieldName = nameof(RawDateData.Day);
            var valueFieldName = nameof(RawDateData.Value);
            
            for (var i = 0; i < dataList.Count; i++)
            {
                var dateData = dataList[i];
                if (dateData == null)
                    continue;

                var rewardStrBuilder = new StringBuilder().Append("\"");
                for (var j = 0; j < dateData.RewardList.Count; j++)
                {
                    var reward = dateData.RewardList[j];
                    if (j != 0)
                        rewardStrBuilder.Append("_");
                    rewardStrBuilder.Append($"{reward.Reward}|{reward.Amount}");
                }
                rewardStrBuilder.Append("\"");
                
                var dateStrBuilder = new StringBuilder()
                    .Append($"{{\"{dayFieldName}\":")
                    .Append($"\"{dateData.Day}\",\"{valueFieldName}\":").
                    Append(rewardStrBuilder.ToString())
                    .Append("}");
 
                strBuilder.Append(dateStrBuilder.ToString());
                if (i != dataList.Count - 1)
                    strBuilder.Append(",");
            }

            strBuilder.Append("]}");
            return strBuilder.ToString();
        }
    }
}