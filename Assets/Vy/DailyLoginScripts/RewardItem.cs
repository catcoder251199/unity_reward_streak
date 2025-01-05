using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace DailyRewards
{
    public class RewardItem : MonoBehaviour
    {
        [SerializeField, Tooltip("reward id")] protected string id; // Reward id
        [SerializeField, Tooltip("reward count")] protected int count; // Reward Count
        public string RewardId => id;
        public int RewardCount => count;

        [SerializeField, AssetReferenceUILabelRestriction("ChestRewardDisplay")] protected AssetReferenceGameObject[] displayAddressList = { };

        /// <summary>
        /// A dictionary that assists in retrieving the address of the corresponding Display Prefab.
        /// </summary>
        protected static readonly Dictionary<string, int> DisplayAddressDictionary
        = new()
        {
            {DailyRewardsConstants.Gold, 0},
            {DailyRewardsConstants.Refresh, 1},
            {DailyRewardsConstants.Clear, 2},
            {DailyRewardsConstants.Sort, 3},
        };
        
        protected static bool IsRewardIdValid(string rewardId) => DisplayAddressDictionary.ContainsKey(rewardId);
        private static bool IsIndexValid<T>(int index, T[] array) => index >= 0 && index < array.Length;
        private bool IsRewardIdValidToDisplay(string rewardId)
        {
            if (!DisplayAddressDictionary.ContainsKey(rewardId) || !IsIndexValid(DisplayAddressDictionary[rewardId], displayAddressList))
                return false;

            return displayAddressList[DisplayAddressDictionary[rewardId]] != null;
        }

        private bool TryGetRewardDisplayAddress(string rewardId, out AssetReferenceGameObject outAddress)
        {
            if (!IsRewardIdValidToDisplay(rewardId))
            {
                outAddress = null;
                return false;
            }

            outAddress = displayAddressList[DisplayAddressDictionary[rewardId]];
            return true;
        }

        [SerializeField] protected Transform displayParentTransform;
        [SerializeField] protected AbstractRewardDisplay currentDisplayIntance;
        protected AsyncOperationHandle<GameObject> LoadDisplayPrefabHandle;

        [ContextMenu("Update Display")]
        public void UpdateDisplayWithCurrentIdAndCount()
        {
            UpdateDisplayWithRewardId(id, count).Forget();
        }

        public async UniTaskVoid UpdateDisplayWithRewardId(string rewardId, int rewardCount)
        {
            if (currentDisplayIntance != null && currentDisplayIntance.RewardId == rewardId)
            {
                count = rewardCount;
                currentDisplayIntance.UpdateRewardCountText(rewardCount);
                //return;
            }
            
            const string logPrefix = "[RewardItem] ";

            Uninitialize();

            id = rewardId;

            if (!TryGetRewardDisplayAddress(rewardId, out AssetReferenceGameObject displayPrefabAddress))
                return;

            string cachedId = id;
            LoadDisplayPrefabHandle = Addressables.LoadAssetAsync<GameObject>(displayPrefabAddress);
            await LoadDisplayPrefabHandle.ToUniTask();

            if (LoadDisplayPrefabHandle.Status == AsyncOperationStatus.Succeeded)
            {
                if (LoadDisplayPrefabHandle.Result.TryGetComponent(out AbstractRewardDisplay displayPrefab))
                {
                    if (rewardId == cachedId)
                    {
                        currentDisplayIntance = Instantiate(displayPrefab, displayParentTransform);
                        currentDisplayIntance.Initialize(rewardId, rewardCount);
                    }
                    else
                    {
                        Uninitialize();
                    }
                }
                else
                {
                    Debug.LogError(logPrefix + "Failed to load display prefab because the prefab does not contain the AbstractChestRewardDisplay component!");
                    ReleaseDisplay();
                }
            }
            else
            {
                Debug.LogError(logPrefix + "Failed to load display prefab from addressable!");

                if (LoadDisplayPrefabHandle.IsValid())
                    Addressables.Release(LoadDisplayPrefabHandle);
            }
        }

        public virtual void Uninitialize()
        {
            id = "";
            count = -1;
            ReleaseDisplay();
        }

        protected virtual void ReleaseDisplay()
        {
            if (currentDisplayIntance != null)
            {
                Destroy(currentDisplayIntance);
                currentDisplayIntance = null;
            }
            
            foreach (Transform child in displayParentTransform) {
                GameObject.Destroy(child.gameObject);
            }

            if (LoadDisplayPrefabHandle.IsValid())
                Addressables.Release(LoadDisplayPrefabHandle);
        }

        protected virtual void Destroy()
        {
            Uninitialize();
        }
    }
}

