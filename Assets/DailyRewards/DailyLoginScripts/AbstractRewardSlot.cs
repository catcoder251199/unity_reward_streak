using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DailyRewards
{
    public abstract class AbstractItemState
    {
        public abstract void Initialize();
        public abstract void OnExit();
        public abstract void OnEnter();
    }

    public abstract class AbstractRewardSlot : MonoBehaviour
    {
        protected DateData dateData;
        protected AbstractItemState _currentState;

        public virtual void SetState(AbstractItemState state)
        {
            if (_currentState == state)
                return;
            _currentState.OnExit();
            _currentState = state;
            _currentState.OnEnter();
        }

        public abstract AbstractItemState GetUnavailableState();
        public abstract AbstractItemState GetReceivedState();
        public abstract AbstractItemState GetReceivableState();
        public abstract void Initialize(DateData data, AbstractItemState initialState);
        public abstract void ClaimRewards(List<RewardItemData> rewardList, bool stillClaimable, bool watchAd);
        public abstract UniTask ClaimRewardsAsync(List<RewardItemData> rewardList, bool stillClaimable, bool watchAd);
        public abstract Vector3 GetCoinbarPosition();
    }
}