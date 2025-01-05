using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
//using Skylink.Analytics; // TODO: uncomment this line
//using Skylink.GameFramework; // TODO: uncomment this line

using UnityEngine;

namespace RewardStreak
{
    public class RewardStreakPopup : MonoBehaviour
    {
        // Tracking
        private enum TrackingInteraction
        {
            ClickClaimFreeReward = 0,
            ClickClaimAdsReward = 1,
        }

        private const string PopupRewardStreakName = "RewardStreakPopup";

        [SerializeField] private bool initOnAwake = false;
        [SerializeField] private UIPopup popup;

        [SerializeField, Header("View/UI")] private RewardStreakItem[] freeItemList;
        [SerializeField, Header("View/UI")] private RewardStreakItem[] adsItemList;
        [SerializeField] RewardStreakProgressUI progressUI;

        public RewardStreakSystem RewardStreakSystem => rewardStreakSystem;
        private LockUI LockUI { get; set; }
        private RewardStreakSystem rewardStreakSystem;

        private void Start()
        {
            if (initOnAwake)
            {
                Initialize(RewardStreakSystem.Instance);
            }
        }

        private void OnDestroy()
        {
            if (rewardStreakSystem != null)
            {
                rewardStreakSystem.OnFreeRewardClaimedEvent -= OnFreeRewardClaimedEventHandler;
                rewardStreakSystem.OnAdsRewardClaimedEvent -= OnAdsRewardClaimedEventHandler;
            }
        }

        public void Initialize(RewardStreakSystem rewardStreakSystem)
        {
            if (rewardStreakSystem == null)
            {
                PrintLogError("rewardStreakSystem is null");
                return;
            }

            this.rewardStreakSystem = rewardStreakSystem;
            LockUI = new LockUI();

            InitializeContent();
            InitializeProgressUI();
        }

        private void InitializeProgressUI()
        {
            //TODO: Implement this method
        }

        private void InitializeContent()
        {
            InitializeItems(freeItemList, rewardStreakSystem.Config.FreeRewards);
            InitializeItems(adsItemList, rewardStreakSystem.Config.AdsRewards);
        }

        private void InitializeItems(RewardStreakItem[] itemArray, List<Reward> rewardList)
        {
            var loopCount = Math.Max(rewardList.Count, itemArray.Length);
            for (var i = 0; i < loopCount; i++)
            {
                
                if (i < itemArray.Length && i < rewardList.Count)
                {
                    itemArray[i].gameObject.SetActive(true);
                    itemArray[i].Initialize(i, rewardList[i]);
                }
                else if (i < freeItemList.Length)
                {
                    itemArray[i].gameObject.SetActive(false);
                }
            }
        }

        public async void OnButtonClosedClicked()
        {
            Debug.Log("Close button clicked");
            //SoundHelper.Instance?.TriggerHaptic(HapticType.Soft); // TODO: uncomment this line
            if (LockUI.IsLocked())
                return;

            LockUI.Lock();
            await UniTask.Delay(RewardStreakConstants.ButtonClickDelay); 
            
            //StaticLock.TryExecute(() =>
            //{
            if (popup != null) popup.Hide();
            //});
        }

        public async void OnFreeButtonClicked(int itemIndex)
        {
            Debug.Log("Free button clicked");
            //SoundHelper.Instance?.TriggerHaptic(HapticType.Soft); // TODO: uncomment this line
            if (LockUI.IsLocked())
                return;

            LockUI.Lock();
            await UniTask.Delay(RewardStreakConstants.ButtonClickDelay);

            //LogActionOnPopupEvent(TrackingInteraction.ClickClaimFreeReward);

            rewardStreakSystem.OnFreeRewardClaimedEvent -= OnFreeRewardClaimedEventHandler;
            rewardStreakSystem.OnFreeRewardClaimedEvent += OnFreeRewardClaimedEventHandler;
            rewardStreakSystem.HandleFreeRewardRequest(itemIndex);
        }

        private async void OnFreeRewardClaimedEventHandler(RewardStreakSystem.ClaimError error, int index, Reward claimedReward)
        {
            if (error == RewardStreakSystem.ClaimError.RewardsUnavailable)
            {
                //TODO: Show error message
                LockUI.UnLock();
                return;
            }

            await ShowReceivedRewardsAsync(index, claimedReward, false);
            LockUI.UnLock();
        }

        private async UniTask ShowReceivedRewardsAsync(int itemIndex, Reward reward, bool watchAd)
        {
            if (reward == null)
                return;

            var itemList = watchAd ? adsItemList : freeItemList;
            var item = itemList[itemIndex];
            
            
            // TODO: Implement this method
            
            //await ClaimRewardsAsync(reward, stillClaimable, watchAd);
        }

        public async void OnAdsButtonClicked(int itemIndex)
        {
            //SoundHelper.Instance?.TriggerHaptic(HapticType.Soft); // TODO: uncomment this line
            Debug.Log("X2 Button Clicked");
            if (LockUI.IsLocked())
                return;

            LockUI.Lock();
            await UniTask.Delay(RewardStreakConstants.ButtonClickDelay);
            
            LogActionOnPopupEvent(TrackingInteraction.ClickClaimAdsReward);
            
            rewardStreakSystem.OnAdsRewardClaimedEvent -= OnAdsRewardClaimedEventHandler;
            rewardStreakSystem.OnAdsRewardClaimedEvent += OnAdsRewardClaimedEventHandler;
            rewardStreakSystem.HandleAdsRewardRequest(itemIndex);
        }

        private async void OnAdsRewardClaimedEventHandler(RewardStreakSystem.ClaimError error, int itemIndex, Reward claimedReward)
        {
            switch (error)
            {
                case RewardStreakSystem.ClaimError.RewardsUnavailable:
                    //TODO: Show error message
                case RewardStreakSystem.ClaimError.WatchAdFailed:
                {
                    //TODO: Show error message
                    LockUI.UnLock();
                    return;
                }
            }

            await ShowReceivedRewardsAsync(itemIndex, claimedReward, false);
            
            LockUI.UnLock();
        }

        private static void PrintLog(string message)
        {
            var logPrefix = $"[{nameof(RewardStreakPopup)}] ";
            Debug.Log(logPrefix + message);
        }

        private static void PrintLogError(string message)
        {
            var logPrefix = $"[{nameof(RewardStreakPopup)}] ";
            Debug.LogError(logPrefix + message);
        }

        private void LogActionOnPopupEvent(TrackingInteraction action)
        {
            // TODO: uncomment this method
            // var actionCode = (int)action;
            // var eventName = TrackingEventConstants.EVENT_NAME;
            // Debug.Log("LogActionOnPopupEvent: " + eventName + " " + actionCode + " " + PopupRewardStreakName);
            //
            // SkylinkAnalytics.LogEvent(eventName, new Dictionary<string, object>()
            // {
            //     { eventName, PopupRewardStreakName },
            //     { TrackingEventConstants.EVENT_PARAM_NAME_ACTION, actionCode.ToString() }
            // });
        }
    }
}