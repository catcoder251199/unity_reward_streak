using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DailyRewards
{
    public class DailyRewardsSystem : MonoBehaviour
    {
        public static DailyRewardsSystem Instance { get; private set; }

        public event Action<TimeSpan> OnTimeUpdatedEvent;
        public event Action OnRewardsClaimedEvent;

        public enum ClaimError
        {
            None,
            RewardsUnavailable,
            WatchAdFailed
        }
        public event Action<ClaimError> OnFreeRewardsClaimedEvent; // OnFreeRewardsClaimedEvent(bool success)
        public event Action<ClaimError> OnDoubleRewardsClaimedEvent; // OnDoubleRewardsClaimedEvent(bool success)

        [SerializeField] private DailyRewardsModel model;
        [SerializeField] private bool initOnAwake = false;

        private DateTime currentLocalDateTime;
        private DateTime startOfTomorrow;

        public DailyRewardsModel Model => model;
        public DailyRewardsConfig Config => model.Config;
        public bool HasAnyClaimableRewards => /*!model.HasClaimedAdRewards || */!model.HasClaimed;
        public bool EnoughLevelToReceiveRewards => model.EnoughLevelToReceiveRewards;
        public TimeSpan TimeLeftUntilRewardDay => model.RewardDateTime - currentLocalDateTime;

        private void Awake()
        {
            if (initOnAwake)
            {
                Initialize();
            }

            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
            }
        }

        public void Initialize()
        {
            model.Initialize();
            CheckOnNewDayHasCome();
            StartTimeCheck();
        }

        private DateTime nextDayTime;

        private void StartTimeCheck()
        {
            StartCoroutine(TimeCheckRoutine());
        }

        private IEnumerator TimeCheckRoutine()
        {
            currentLocalDateTime = DailyRewardsHelper.GetCurrentLocalDateTime();
            var lastCurrentDateTime = currentLocalDateTime;
            startOfTomorrow = currentLocalDateTime.Date.AddDays(1);

            do
            {
                if (currentLocalDateTime.Date > lastCurrentDateTime.Date)
                {
                    CheckOnNewDayHasCome();
                }

                var timeLeftForNextDay = startOfTomorrow - currentLocalDateTime;
                OnTimeUpdatedEvent?.Invoke(timeLeftForNextDay);
                lastCurrentDateTime = currentLocalDateTime;

                yield return new WaitForSeconds(1f);

                currentLocalDateTime = DailyRewardsHelper.GetCurrentLocalDateTime();
                startOfTomorrow = currentLocalDateTime.Date.AddDays(1);
            } while (true);
        }

        public void CheckOnNewDayHasCome()
        {
            var currentDateTime = DailyRewardsHelper.GetCurrentLocalDateTime();
            var rewardDateTime = model.RewardDateTime; // The date when the user is eligible to receive a reward.
            var isRewardDayPassed = currentDateTime.Date > rewardDateTime.Date;
            var claimedAnyRewards = model.HasClaimedAnyRewards;

            if (!isRewardDayPassed) return;

            if (claimedAnyRewards)
            {
                model.IncreaseCurrentDayIndex();
                model.ResetClaimedRewardsFlags();
            }

            model.SetRewardDateTime(DailyRewardsHelper.GetStartOfTodayAsLong());
        }

        public void HandleFreeButtonClick()
        {
            Debug.Log("system handle free button click");
            var success = TryClaimFreeRewards();
            if (!success)
            {
                OnFreeRewardsClaimedEvent?.Invoke(ClaimError.RewardsUnavailable);
                return;
            }
            
            OnFreeRewardsClaimedEvent?.Invoke(ClaimError.None);
        }
        
        private bool TryClaimFreeRewards()
        {
            if (!CanClaimFreeRewards())
                return false;
            model.SetClaimedRewards(true);
            return true;
        }

        public void HandleX2ButtonClick()
        {
            Debug.Log("system handle x2 button click");
            if (!CanClaimX2Rewards())
            {
                OnDoubleRewardsClaimedEvent?.Invoke(ClaimError.RewardsUnavailable);
                return;
            }
            
            GameMaster.Instance.ShowRewardedVideo(OnDoubleRewardsWatchAdSuccess, OnDoubleRewardsWatchAdFailed, "DailyRewardsX2").Forget();
        }
        
        private void OnDoubleRewardsWatchAdSuccess()
        {
            OnDoubleRewardsClaimedEvent?.Invoke(ClaimError.None);
        }
        
        private void OnDoubleRewardsWatchAdFailed()
        {
            OnDoubleRewardsClaimedEvent?.Invoke(ClaimError.WatchAdFailed);
        }
        
        private async UniTask WatchAdAsync(Action onSuccess, Action<string> onFailed, string adPlacement)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.5));
            onSuccess?.Invoke();
        }

        public bool CanClaimX2Rewards()
        {
            return model.DoesTodayHaveRewards && !model.HasClaimed;
        }

        public bool CanClaimFreeRewards()
        {
            return model.DoesTodayHaveRewards && !model.HasClaimed;
        }

        private bool TryClaimDoubleRewards()
        {
            if (!CanClaimX2Rewards()) return false;
            model.SetClaimedRewards(true);
            return true;
        }

        public List<RewardItemData> GetRewardsToday()
        {
            var day = model.CurrentDay;
            var dayIndex = day - 1;
            var rewardList = model.Config.RewardList;
            var rewardData = 0 < dayIndex && dayIndex < rewardList.Count ? rewardList[dayIndex] : null;
            if (rewardData == null)
                rewardData = rewardList.Find(data => data.Day == day);

            return rewardData?.RewardList;
        }

        public List<RewardItemData> GetDoubledRewardsToday()
        {
            var day = model.CurrentDay;
            var dayIndex = day - 1;
            var rewardList = model.Config.RewardList;
            var rewardData = 0 < dayIndex && dayIndex < rewardList.Count ? rewardList[dayIndex] : null;
            if (rewardData == null)
                rewardData = rewardList.Find(data => data.Day == day);

            var ret = new List<RewardItemData>();
            foreach (var reward in rewardData.RewardList)
                ret.Add(new RewardItemData(reward.Reward, reward.Amount * 2));

            return ret;
        }
    }
}