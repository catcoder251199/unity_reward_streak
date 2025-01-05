using System;
using Skylink.GameFramework;
using TMPro;
using UnityEngine;

namespace DailyRewards
{
    public class DailyRewardsTimerPanel : MonoBehaviour
    {
        private static DailyRewardsSystem TryGetDailyRewardsSystem() => DailyRewardsSystem.Instance;

        [SerializeField] private GameObject timerPanel;
        [SerializeField] private TextMeshProUGUI timerText;
        
        private void Start()
        {
            var dailyRewardsSystem = TryGetDailyRewardsSystem();
            
            if (dailyRewardsSystem == null)
                dailyRewardsSystem = DailyRewardsSystem.Instance;

            if (dailyRewardsSystem == null || !dailyRewardsSystem.EnoughLevelToReceiveRewards)
            {
                timerPanel.gameObject.SetActive(false);
                timerText.gameObject.SetActive(false);
                return;
            }

            UpdateTimer();
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
            UpdateTimer();
        }

        private void OnTimerTickedEventHandler(TimeSpan timeLeftForNextDay)
        {
            UpdateTimer();
        }

        private void UpdateTimer()
        {
            var dailyRewardsSystem = TryGetDailyRewardsSystem();
            var secondsUntilRewardTime =
                Mathf.FloorToInt((float)dailyRewardsSystem.TimeLeftUntilRewardDay.TotalSeconds);
            timerPanel.gameObject.SetActive(secondsUntilRewardTime >= 0);

             if (timerPanel.gameObject.activeSelf)
             {
                 timerText.text = $"{CommonUtils.GetTimerString(secondsUntilRewardTime)}";
             }
        }
    }
}