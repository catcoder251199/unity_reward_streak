using System;
using UnityEngine;

namespace DailyRewards
{
    public class DailyRewardsModel : MonoBehaviour
    {
        [SerializeField] private AbstractDailyRewardsStorage userStorage;
        [SerializeField] private DailyRewardsConfig config;
        
        public DailyRewardsConfig Config => config;
        public int CurrentDay => (userStorage.DailyRewardsUserData.currentDay % config.TotalDays) + 1;
        public DateTime RewardDateTime =>
            DailyRewardsHelper.ConvertLongToDateTime(userStorage.DailyRewardsUserData.rewardDateTime);
        public bool HasClaimed => userStorage.DailyRewardsUserData.claimed;
        //public bool HasClaimedAdRewards => userStorage.DailyRewardsUserData.claimedAdRewards;
        public bool HasClaimedAnyRewards => HasClaimed /*|| HasClaimedAdRewards*/;
        public bool HasClaimedAllRewards => HasClaimed /*&& HasClaimedAdRewards*/;
        public bool DoesTodayHaveRewards => DailyRewardsHelper.GetCurrentLocalDateTime().Date == RewardDateTime.Date;
        public bool EnoughLevelToReceiveRewards => userStorage.CurrentLevel >= config.MinLevelToReceiveRewards;

        public void Initialize()
        {
            InitializeUserStorage();
            InitializeConfig();
        }

        private void InitializeConfig()
        {
            config.LoadConfigFromRemote();
        }

        private void InitializeUserStorage()
        {
            userStorage.Initialize();
        }

        public void IncreaseCurrentDayIndex()
        {
            userStorage.DailyRewardsUserData.currentDay++;
            userStorage.MarkDirty();
        }

        public void SetRewardDateTime(long rewardDateTime)
        {
            userStorage.DailyRewardsUserData.rewardDateTime = rewardDateTime;
            userStorage.MarkDirty();
        }


        public void ResetClaimedRewardsFlags()
        {
            userStorage.DailyRewardsUserData.claimed = false;
            userStorage.MarkDirty();
        }

        public void SetClaimedRewards(bool claimed)
        {
            userStorage.DailyRewardsUserData.claimed = claimed;
            if (HasClaimedAllRewards)
            {
                userStorage.DailyRewardsUserData.currentDay++;
                userStorage.DailyRewardsUserData.claimed = false;
                userStorage.DailyRewardsUserData.rewardDateTime = DailyRewardsHelper.GetStartTomorrowAsLong();
            }

            userStorage.MarkDirty();
        }
    }
}