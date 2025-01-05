using System;
using Skylink.GameFramework;
using TMPro;
using UnityEngine;

namespace DailyRewards
{
   public class TimerUIController : AbstractTimerUIController
   {
       [SerializeField] private GameObject timerPanel;
       [SerializeField] private TextMeshProUGUI timerText;

       private DailyRewardsSystem dailyRewardsSystem;
       
       public override void Initialize(DailyRewardsSystem dailyRewardsSystem)
       {
           this.dailyRewardsSystem = dailyRewardsSystem;
           timerPanel.SetActive(false);
       }
       public override void UpdateTimer()
       {
           var currentLocalDateTime = DailyRewardsHelper.GetCurrentLocalDateTime().Date;
           var dataModel = dailyRewardsSystem.Model;
           timerPanel.gameObject.SetActive(currentLocalDateTime.Date < dataModel.RewardDateTime.Date);
           if (timerPanel.gameObject.activeSelf)
           {
               OnTimerTicked(dailyRewardsSystem.TimeLeftUntilRewardDay);
               dailyRewardsSystem.OnTimeUpdatedEvent -= OnTimerTicked;
               dailyRewardsSystem.OnTimeUpdatedEvent += OnTimerTicked;
           }
           else
           {
               dailyRewardsSystem.OnTimeUpdatedEvent -= OnTimerTicked;
           }
       }
       
       private void OnTimerTicked(TimeSpan timeUntilTomorrow)
       {
           var secondsUntilRewardTime = Mathf.FloorToInt((float)dailyRewardsSystem.TimeLeftUntilRewardDay.TotalSeconds);
           timerPanel.gameObject.SetActive(secondsUntilRewardTime >= 0);

           if (secondsUntilRewardTime < 0)
           {
               dailyRewardsSystem.OnTimeUpdatedEvent -= OnTimerTicked;
               timerPanel.gameObject.SetActive(false);
           }

           if (timerPanel.gameObject.activeSelf)
           {
               timerText.text = GetFormattedTime(secondsUntilRewardTime);
           }
       }

       private static string GetFormattedTime(int secondsUntilRewardTime) => CommonUtils.GetTimerString(secondsUntilRewardTime);
       
       private void OnDestroy()
       {
           dailyRewardsSystem.OnTimeUpdatedEvent -= OnTimerTicked;
       }
   } 
}