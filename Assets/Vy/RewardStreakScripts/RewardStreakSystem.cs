using System;
using System.Collections;
using UnityEngine;
using RewardStreak;
using Reward = RewardStreak.Reward;

namespace RewardStreak
{
    public class RewardStreakSystem : /*SingletonMonoNoAutoGen<RewardStreakSystem>*/ MonoBehaviour
    {
        #region Singleton

        public static RewardStreakSystem Instance { get; private set; }

        #endregion

        public event Action<TimeSpan> OnFreeRewardTimeUpdatedEvent;
        public event Action<TimeSpan> OnAdsRewardTimeUpdatedEvent;
        public event Action<ClaimError, int, Reward> OnFreeRewardClaimedEvent;
        public event Action<ClaimError, int, Reward> OnAdsRewardClaimedEvent;

        public enum ClaimError
        {
            None,
            RewardsUnavailable,
            WatchAdFailed,
        }

        [SerializeField] private RewardStreakModel model;
        [SerializeField] private bool initOnAwake = false;

        private DateTime currentLocalDateTime;
        private DateTime startOfTomorrow;
        private DateTime nextDayTime;

        public RewardStreakModel Model => model;
        public RewardStreakConfigSO Config => model.Config;

        // Player Prefs
        public void SaveLastLoggedInTime(string key, long time) => PlayerPrefs.SetString(key, time.ToString());

        public long GetLastLoggedInTime(string key)
        {
            var nextTimeStr = PlayerPrefs.GetString(key, "0");
            return long.TryParse(nextTimeStr, out long nextTime) ? nextTime : 0;
        }

        public bool IsFirstLogin()
        {
            var lastLoggedInTime = GetLastLoggedInTime(PlayerPrefsKeys.REWARD_STREAK_LAST_SHOWED_POPUP_KEY);
            var lastLoggedInDateTime = DailyRewardsHelper.ConvertLongToDateTime(lastLoggedInTime);
            return lastLoggedInDateTime.Date < DailyRewardsHelper.GetCurrentLocalDateTime().Date;
        }

        private void Awake()
        {
            if (initOnAwake)
            {
                Initialize();
            }
        }

        public void Initialize()
        {
            model.Initialize();
            PreprocessData();
            StartTimeCheck();
        }

        private void StartTimeCheck()
        {
            StartCoroutine(TimeCheckRoutine());
        }

        private IEnumerator TimeCheckRoutine()
        {
            do
            {
                var remainingFreeRewardTime = model.GetRemainingFreeRewardTime();
                OnFreeRewardTimeUpdatedEvent?.Invoke(remainingFreeRewardTime);

                var remainingAdsRewardTime = model.GetRemainingAdsRewardTime();
                OnAdsRewardTimeUpdatedEvent?.Invoke(remainingAdsRewardTime);

                if (remainingFreeRewardTime.Seconds <= 0)
                    model.ResetFreeRewardStreak();

                if (remainingAdsRewardTime.Seconds <= 0)
                    model.ResetAdsRewardStreak();

                yield return new WaitForSeconds(1f);
            } while (true);
        }

        private void PreprocessData()
        {
            var remainingFreeRewardTime = model.GetRemainingFreeRewardTime();
            if (remainingFreeRewardTime.Seconds <= 0)
                model.ResetFreeRewardStreak();

            var remainingAdsRewardTime = model.GetRemainingAdsRewardTime();
            if (remainingAdsRewardTime.Seconds <= 0)
                model.ResetAdsRewardStreak();

            model.ValidateFreeRewardStreak();
            model.ValidateAdsRewardStreak();
        }

        public void HandleFreeRewardRequest(int rewardIndex)
        {
            if (!CanClaimFreeRewards())
            {
                OnFreeRewardClaimedEvent?.Invoke(ClaimError.RewardsUnavailable, rewardIndex, null);
                return;
            }

            var reward = GetFreeReward(rewardIndex);
            model.UpdateOnClaimedFreeReward();
            OnAdsRewardClaimedEvent?.Invoke(reward != null ? ClaimError.None : ClaimError.RewardsUnavailable,
                rewardIndex, reward);
        }

        public void HandleAdsRewardRequest(int rewardIndex)
        {
            if (!CanClaimAdsReward())
            {
                OnAdsRewardClaimedEvent?.Invoke(ClaimError.RewardsUnavailable, rewardIndex, null);
                return;
            }

            GameMaster.Instance.ShowRewardedVideo(
                () => OnClaimAdsRewardSuccess(rewardIndex),
                (string error) => OnClaimAdsRewardFailed(error, rewardIndex),
                RewardStreakConstants.RewardStreakAdsLocation);
        }

        private void OnClaimAdsRewardSuccess(int rewardIndex)
        {
            var reward = GetAdsReward(rewardIndex);
            model.UpdateOnClaimedAdsReward();
            OnAdsRewardClaimedEvent?.Invoke(reward != null ? ClaimError.None : ClaimError.RewardsUnavailable,
                rewardIndex, reward);
        }

        private void OnClaimAdsRewardFailed(string error, int rewardIndex)
        {
            OnAdsRewardClaimedEvent?.Invoke(ClaimError.WatchAdFailed, rewardIndex, null);
        }

        private bool CanClaimAdsReward()
        {
            return !model.HasReachedMaxAdsStreak;
        }

        private bool CanClaimFreeRewards()
        {
            return !model.HasReachedMaxFreeStreak;
        }

        private Reward GetFreeReward(int currentIndex)
        {
            var rewardList = model.Config.FreeRewards;
            if (rewardList == null || rewardList.Count == 0) return null;
            if (currentIndex < 0 || currentIndex >= rewardList.Count) return null;
            return rewardList[currentIndex];
        }

        private Reward GetAdsReward(int currentIndex)
        {
            var rewardList = model.Config.AdsRewards;
            if (rewardList == null || rewardList.Count == 0) return null;
            if (currentIndex < 0 || currentIndex >= rewardList.Count) return null;
            return rewardList[currentIndex];
        }
    }
}