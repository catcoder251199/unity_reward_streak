using System;
using UnityEngine;

namespace DailyRewards
{
    public class DailyRewardsRedDot : MonoBehaviour
    {
        private static DailyRewardsSystem TryGetDailyRewardsSystem() => DailyRewardsSystem.Instance;
        private void Start()
        {
            var dailyRewardsSystem = TryGetDailyRewardsSystem();

            if (dailyRewardsSystem == null || !dailyRewardsSystem.EnoughLevelToReceiveRewards)
                return;

            UpdateRedDot();

            dailyRewardsSystem.OnTimeUpdatedEvent += OnTimerTickedEventHandler;
            dailyRewardsSystem.OnRewardsClaimedEvent += OnRewardsClaimedEventHandler;
        }

        private void OnDestroy()
        {
            var dailyRewardsSystem = TryGetDailyRewardsSystem();
            if (dailyRewardsSystem != null)
            {
                dailyRewardsSystem.OnTimeUpdatedEvent -= OnTimerTickedEventHandler;
                dailyRewardsSystem.OnRewardsClaimedEvent -= OnRewardsClaimedEventHandler;
            }
        }

        private void OnRewardsClaimedEventHandler()
        {
            UpdateRedDot();
        }

        private void OnTimerTickedEventHandler(TimeSpan timeLeftForNextDay)
        {
            UpdateRedDot();
        }

        private void UpdateRedDot()
        {
            var dailyRewardsSystem = TryGetDailyRewardsSystem();
            var doesTodayHaveRewards = dailyRewardsSystem.Model.DoesTodayHaveRewards;
            var canClaimSomething = dailyRewardsSystem.HasAnyClaimableRewards;
            gameObject.SetActive(canClaimSomething && doesTodayHaveRewards);
        }
    }
}