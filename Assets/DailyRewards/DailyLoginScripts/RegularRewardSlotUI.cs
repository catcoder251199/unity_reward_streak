using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace DailyRewards
{
    public partial class RegularRewardSlotUI : AbstractRewardSlot
    {
        [SerializeField, Header("UI elements")]
        protected GameObject receivedFrontPanel;

        [SerializeField] protected GameObject receivedBackPanel;

        [SerializeField] protected GameObject receivablePanel;

        //[SerializeField] protected GameObject unavailablePanel;
        [SerializeField] protected RewardItem rewardItem;
        [SerializeField] private TextMeshProUGUI dayText;

        [SerializeField, Header("Animation")] private Animator animator;
        [SerializeField] protected string animationClaimed;

        [SerializeField] private Transform spawningRewardsTransform;
        [SerializeField] private Transform coinBarTransform;

        public override void Initialize(DateData data, AbstractItemState initialState)
        {
            dateData = data;
            _currentState = initialState;
            _currentState.Initialize();
        }

        public override void ClaimRewards(List<RewardItemData> rewardList, bool stillClaimable, bool watchAd)
        {
            if (rewardList == null || rewardList.Count == 0)
                return;

            foreach (var rewardInfo in rewardList)
            {
                SpawnReward(rewardInfo.Reward, rewardInfo.Amount,
                    watchAd); // TODO: FlyersManager should have option delay instead of using await
            }

            if (!stillClaimable)
                SetState(GetReceivedState());
        }

        public override async UniTask ClaimRewardsAsync(List<RewardItemData> rewardList, bool stillClaimable,
            bool watchAd)
        {
            if (rewardList == null || rewardList.Count == 0)
                return;

            if (!stillClaimable)
                SetState(GetReceivedState());
            
            var rewardInfo = rewardList[0];
            await SpawnRewardAsync(rewardInfo.Reward, rewardInfo.Amount,
                watchAd);
        }

        private void SpawnReward(string rewardId, int amount, bool watchAd)
        {
            var spawningPosition = spawningRewardsTransform?.position ?? Vector3.zero;
            var destinationPosition = GetCoinbarPosition();
            DailyRewardsHelper.SpawnItem(rewardId, amount, spawningPosition, destinationPosition, watchAd);
        }
        
        private async UniTask SpawnRewardAsync(string rewardId, int amount, bool watchAd)
        {
            //var spawningPosition = spawningRewardsTransform?.position ?? Vector3.zero;
            var spawningPosition = Vector3.zero;
            var destinationPosition = GetCoinbarPosition();
            await DailyRewardsHelper.SpawnItemAsync(rewardId, amount, spawningPosition, destinationPosition, watchAd);
        }

        public override Vector3 GetCoinbarPosition() => /*coinBarTransform?.position ?? Vector3.zero;*/ Vector3.zero;

        public override AbstractItemState GetReceivableState() => new RegularReceivableState(this);
        public override AbstractItemState GetReceivedState() => new RegularReceivedState(this);
        public override AbstractItemState GetUnavailableState() => new RegularUnavailableState(this);
    }
}

namespace DailyRewards
{
    public partial class RegularRewardSlotUI
    {
        private abstract class RegularAbstractState : AbstractItemState
        {
            protected readonly RegularRewardSlotUI Context;

            protected RegularAbstractState(RegularRewardSlotUI context)
            {
                Context = context;
            }

            public virtual void UpdateContent()
            {
                Context.dayText.text = "Day " + Context.dateData.Day;
                var rewardData = Context.dateData.RewardList[0];
                Context.rewardItem.UpdateDisplayWithRewardId(rewardData.Reward, rewardData.Amount).Forget();
            }
        }

        private class RegularReceivableState : RegularAbstractState
        {
            public RegularReceivableState(RegularRewardSlotUI context) : base(context)
            {
            }

            public override void Initialize() => UpdateContent();

            public override void OnEnter() => UpdateContent();

            public override void OnExit()
            {
            }

            public override void UpdateContent()
            {
                base.UpdateContent();

                Context.receivablePanel.SetActive(true);
            }
        }

        private class RegularReceivedState : RegularAbstractState
        {
            public RegularReceivedState(RegularRewardSlotUI context) : base(context)
            {
            }

            public override void Initialize() => UpdateContent();

            public override void OnEnter()
            {
                Context.animator.SetTrigger(Context.animationClaimed);
            }

            public override void OnExit()
            {
            }

            public override void UpdateContent()
            {
                base.UpdateContent();

                Context.receivedFrontPanel.SetActive(true);
                Context.receivedBackPanel.SetActive(true);
            }
        }

        private class RegularUnavailableState : RegularAbstractState
        {
            public RegularUnavailableState(RegularRewardSlotUI context) : base(context)
            {
            }

            public override void Initialize() => UpdateContent();

            public override void OnEnter()
            {
            }

            public override void OnExit()
            {
            }

            public override void UpdateContent()
            {
                base.UpdateContent();
                Context.receivedBackPanel.SetActive(true);
            }
        }
    }
}