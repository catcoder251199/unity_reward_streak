
using System;
using UnityEngine;

namespace RewardStreak
{
    public class RewardStreakModel : MonoBehaviour
    {
        [SerializeField] private AbstractRewardStreakStorage userStorage;
        [SerializeField] private RewardStreakConfigSO configSO;
        
        public RewardStreakConfigSO Config => configSO;

        public TimeSpan GetRemainingFreeRewardTime()
        {
            var currentTime = RewardStreakHelper.GetCurrentTimeUtcInSeconds();
            var nextClaimTime = userStorage.UserData.lastClaimedFreeTime + configSO.FreeRewardCooldown; // in seconds
            return TimeSpan.FromSeconds(nextClaimTime - currentTime);
        }
        
        public TimeSpan GetRemainingAdsRewardTime()
        {
            var currentTime = RewardStreakHelper.GetCurrentTimeUtcInSeconds();
            var nextClaimTime = userStorage.UserData.lastClaimedAdsTime + configSO.FreeRewardCooldown; // in seconds
            return TimeSpan.FromSeconds(nextClaimTime - currentTime);
        }

        public TimeSpan RemainingAdRewardTime => TimeSpan.Zero; // TODO: Implement this
        public bool EnoughLevelForRewardStreak => userStorage.CurrentLevel >= configSO.MinLevelRewardStreak;
        public bool HasReachedMaxAdsStreak => userStorage.UserData.adsRewardsStreak >= configSO.AdsRewards.Count;
        public bool HasReachedMaxFreeStreak => userStorage.UserData.freeRewardStreak >= configSO.FreeRewards.Count;

        public void ResetFreeRewardStreak()
        {
            userStorage.UserData.freeRewardStreak = 0;
            userStorage.MarkDirty();
        }
        
        public void ResetAdsRewardStreak()
        {
            userStorage.UserData.adsRewardsStreak = 0;
            userStorage.MarkDirty();
        }

        public void UpdateOnClaimedAdsReward()
        {
            var previousStreak = userStorage.UserData.adsRewardsStreak;
            var currentStreak = Math.Clamp(userStorage.UserData.adsRewardsStreak + 1, 0, configSO.AdsRewards.Count);
            userStorage.UserData.adsRewardsStreak = currentStreak;

            if (previousStreak == 0 && currentStreak > 0) // if it's first time in streak to claim reward
            {
                userStorage.UserData.lastClaimedAdsTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            }
        }
        
        public void UpdateOnClaimedFreeReward()
        {
            var previousStreak = userStorage.UserData.freeRewardStreak;
            var currentStreak = Math.Clamp(userStorage.UserData.freeRewardStreak + 1, 0, configSO.FreeRewards.Count);
            userStorage.UserData.adsRewardsStreak = currentStreak;

            if (previousStreak == 0 && currentStreak > 0) // if it's first time in streak to claim reward
            {
                userStorage.UserData.lastClaimedFreeTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            }
        }

        public void Initialize()
        {
            InitializeUserStorage();
            InitializeConfig();
        }

        private void InitializeConfig()
        {
            configSO.LoadConfigFromRemote();
        }

        private void InitializeUserStorage()
        {
            userStorage.Initialize();
        }

        public void ValidateFreeRewardStreak()
        {
            var previousStreak = userStorage.UserData.freeRewardStreak;
            var currentStreak = Math.Clamp(userStorage.UserData.freeRewardStreak + 1, 0, configSO.FreeRewards.Count);
            if (previousStreak != currentStreak)
                userStorage.MarkDirty();
        }

        public void ValidateAdsRewardStreak()
        {
            var previousStreak = userStorage.UserData.adsRewardsStreak;
            var currentStreak = Math.Clamp(userStorage.UserData.adsRewardsStreak + 1, 0, configSO.AdsRewards.Count);
            if (previousStreak != currentStreak)
                userStorage.MarkDirty();
        }
    }
}