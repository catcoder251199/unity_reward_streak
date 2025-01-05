using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using UnityEngine;

namespace DailyRewards
{
    public class DailyRewardsPopup : MonoBehaviour
    {
        [SerializeField] private bool initOnAwake = false;
        [SerializeField] private UIPopup popup;

        [SerializeField, Header("View/UI")] private DailyRewardsContent content;
        [SerializeField] AbstractButtonsUIController buttonsUIController;
        [SerializeField] AbstractTimerUIController timerUIController;

        public DailyRewardsSystem DailyRewardsSystem => dailyRewardsSystem;
        private AbstractButtonsUIController ButtonsUIController => buttonsUIController;
        private LockUI LockUI { get; set; }

        private DailyRewardsSystem dailyRewardsSystem;
        private DailyRewardsModel dataModel;

        private void Start()
        {
            if (initOnAwake)
            {
                Initialize(DailyRewardsSystem.Instance);
            }
        }

        private void OnDestroy()
        {
            if (dailyRewardsSystem != null)
            {
                dailyRewardsSystem.OnFreeRewardsClaimedEvent -= OnFreeRewardsClaimedEventHandler;
                dailyRewardsSystem.OnDoubleRewardsClaimedEvent -= OnDoubleRewardsClaimedEventHandler;
            }
        }

        public void Initialize(DailyRewardsSystem dailyRewardsSystem)
        {
            if (dailyRewardsSystem == null)
            {
                PrintLogError("dailyRewardsSystem is null");
                return;
            }

            this.dailyRewardsSystem = dailyRewardsSystem;
            dataModel = this.dailyRewardsSystem.Model;
            LockUI = new LockUI();

            InitializeContent();
            InitializeTimerUIController();
            InitializeButtonsUIController();
        }

        private void InitializeTimerUIController()
        {
            if (timerUIController == null)
            {
                PrintLog("timerUIController is null");
                return;
            }

            timerUIController?.Initialize(dailyRewardsSystem);
            timerUIController?.UpdateTimer();
        }

        private void InitializeButtonsUIController()
        {
            if (buttonsUIController == null)
            {
                PrintLog("ButtonsUIController is null");
                return;
            }

            buttonsUIController?.Initialize(dailyRewardsSystem.Model);
            buttonsUIController?.UpdateButtons();
        }

        private void InitializeContent()
        {
            content.Initialize(this);
        }

        public void OnButtonClosedClicked()
        {
            if (LockUI.IsLocked())
                return;

            // StaticLock.TryExecute(() =>
            // {
            //     if (popup != null) popup.Hide();
            // });
        }

        public void OnFreeButtonClicked()
        {
            Debug.Log("Free button clicked");
            if (LockUI.IsLocked())
                return;

            LockUI.Lock();

            // StaticLock.TryExecute(() =>
            // {
            dailyRewardsSystem.OnFreeRewardsClaimedEvent -= OnFreeRewardsClaimedEventHandler;
            dailyRewardsSystem.OnFreeRewardsClaimedEvent += OnFreeRewardsClaimedEventHandler;
            dailyRewardsSystem.HandleFreeButtonClick();
            // });
        }

        private async void OnFreeRewardsClaimedEventHandler(DailyRewardsSystem.ClaimError error)
        {
            if (error == DailyRewardsSystem.ClaimError.RewardsUnavailable)
            {
                ButtonsUIController.SetAllClaimButtonsActive(false);
                ButtonsUIController.SetCloseButton(true);
                LockUI.UnLock();
                return;
            }

            var rewardsToday = dailyRewardsSystem.GetRewardsToday();
            ButtonsUIController.UpdateButtons();
            await ShowReceivedRewardsAsync(rewardsToday, false);
            LockUI.UnLock();
        }

        public async UniTask ShowReceivedRewardsAsync(List<RewardItemData> rewardList, bool watchAd)
        {
            if (rewardList == null || rewardList.Count == 0)
                return;

            if (content.ReceivableAbstractRewardSlot == null) return;

            var doesTodayHaveRewards = dataModel.DoesTodayHaveRewards;
            var stillClaimable = doesTodayHaveRewards && !dataModel.HasClaimed;
            await content.ReceivableAbstractRewardSlot.ClaimRewardsAsync(rewardList, stillClaimable, watchAd);
        }

        public void OnX2ButtonClicked()
        {
            if (LockUI.IsLocked())
                return;

            LockUI.Lock();

            // StaticLock.TryExecute(() =>
            // {
            dailyRewardsSystem.OnDoubleRewardsClaimedEvent -= OnDoubleRewardsClaimedEventHandler;
            dailyRewardsSystem.OnDoubleRewardsClaimedEvent += OnDoubleRewardsClaimedEventHandler;
            dailyRewardsSystem.HandleX2ButtonClick();
            // });
        }
        
        private async void OnDoubleRewardsClaimedEventHandler(DailyRewardsSystem.ClaimError error)
        {
            switch (error)
            {
                case DailyRewardsSystem.ClaimError.RewardsUnavailable:
                {
                    ButtonsUIController.SetAllClaimButtonsActive(false);
                    ButtonsUIController.SetCloseButton(true);
                    LockUI.UnLock();
                    return;
                }
                case DailyRewardsSystem.ClaimError.WatchAdFailed:
                {
                    OnWatchAdFailed();
                    return;
                }
            }
            
            var rewardsToday = dailyRewardsSystem.GetDoubledRewardsToday();
            ButtonsUIController.UpdateButtons();
            await ShowReceivedRewardsAsync(rewardsToday, false);
            LockUI.UnLock();
        }

        private void OnWatchAdFailed()
        {
            LockUI.UnLock();

            //Debug.LogError($"[{nameof(DailyRewardsPopup)}] OnWatchAdFailed: {error}");
            // var uiPopup = UIPopupManager.ShowPopup(UINameConstants.WatchAdFailedPopup, false, false);
            // if (uiPopup != null && uiPopup.TryGetComponent<WatchAdFailedPopup>(out var watchAdFailedPopup))
            // {
            //     watchAdFailedPopup.OnCloseButtonClickedEvent += () =>
            //     {
            //         if (this == null)
            //             return;
            //
            //         lockUI = false;
            //     };
            // }
            // else
            // {
            //     lockUI = false;
            // }
        }

        private static void PrintLog(string message)
        {
            var logPrefix = $"[{nameof(DailyRewardsPopup)}] ";
            Debug.Log(logPrefix + message);
        }

        private static void PrintLogError(string message)
        {
            var logPrefix = $"[{nameof(DailyRewardsPopup)}] ";
            Debug.LogError(logPrefix + message);
        }
    }
}