using System;
using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using TMPro;
using UnityEngine;

namespace DailyRewards
{
    public class DailyRewardsButton : MonoBehaviour
    {
        [SerializeField] private DailyRewardsSystem dailyRewardsSystem;
        
        private UIPopup uiPopup;

        public async UniTask OnHomeEnter()
        {
            if (dailyRewardsSystem == null)
                dailyRewardsSystem = DailyRewardsSystem.Instance;

            if (dailyRewardsSystem == null || !dailyRewardsSystem.EnoughLevelToReceiveRewards)
            {
                gameObject.SetActive(false);
                //timerPanel.SetActive(false);
                return;
            }

            var currentLocalDateTime = DailyRewardsHelper.GetCurrentLocalDateTime().Date;
            var rewardDateTime = dailyRewardsSystem.Model.RewardDateTime.Date;
            var isTodayHaveRewards = currentLocalDateTime.Date == rewardDateTime.Date;
            if (isTodayHaveRewards && !dailyRewardsSystem.Model.HasClaimed)
            {
                OpenDailyRewards();
                await UniTask.WaitUntil(() => uiPopup != null && uiPopup.IsVisible);
                await UniTask.WaitUntil(() => uiPopup != null && uiPopup.IsHidden);
            }
        }

        [ContextMenu("Open Daily Rewards")]
        public void OnButtonPressed()
        {
            OpenDailyRewardsByButton();
        }

        private const string DailyRewardsPopupName = UINameConstants.PopupDailyRewards;

        private void OpenDailyRewardsByButton()
        {
            // StaticLock.TryExecute(() =>
            // {
            uiPopup = UIPopupManager.ShowPopup(DailyRewardsPopupName, true, false);
            if (uiPopup != null && uiPopup.TryGetComponent(out DailyRewardsPopup dailyRewardsUI))
            {
                dailyRewardsUI.Initialize(dailyRewardsSystem);
            }
            // });
        }

        private void OpenDailyRewards()
        {
            uiPopup = UIPopupManager.ShowPopup(DailyRewardsPopupName, true, false);
            if (uiPopup != null && uiPopup.TryGetComponent(out DailyRewardsPopup dailyRewardsUI))
            {
                dailyRewardsUI.Initialize(dailyRewardsSystem);
            }
        }
    }
}