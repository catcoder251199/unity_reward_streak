using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace DailyRewards
{
    public partial class SpecialRewardSlot : AbstractRewardSlot
    {
        [SerializeField, Header("UI elements")] protected GameObject receivedFrontPanel;
        [SerializeField] protected GameObject receivedBackPanel;
        [SerializeField] protected GameObject receivablePanel;
        [SerializeField] protected Transform rewardParent;
        [SerializeField] protected TextMeshProUGUI dayText;
        [SerializeField] protected Transform coinBarTransform;

        [SerializeField, Header("Animation")] private Animator animator;
        [SerializeField] protected string animationClaimed;

        [SerializeField, Header("Rewards")] protected List<RewardItem> rewardItems;
        [SerializeField] protected RewardItem rewardItemPrefab;

        public override AbstractItemState GetUnavailableState() => new SpecialUnavailableState(this);
        public override AbstractItemState GetReceivedState() => new SpecialReceivedState(this);
        public override AbstractItemState GetReceivableState() => new SpecialReceivableState(this);

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

            for (int i = 0, j = -1; i < rewardList.Count; i++)
            {
                if (j + 1 < rewardItems.Count)
                    j++;

                var spawnPosition = rewardParent?.position ?? Vector3.zero;
                if (0 <= j && j < rewardItems.Count)
                    spawnPosition = rewardItems[j].transform.position;

                var rewardInfo = rewardList[i];
                DailyRewardsHelper.SpawnItem(rewardInfo.Reward, rewardInfo.Amount, spawnPosition,
                    coinBarTransform.position, watchAd);
            }

            if (!stillClaimable)
                SetState(GetReceivedState());
        }
        
        public override async UniTask ClaimRewardsAsync(List<RewardItemData> rewardList, bool stillClaimable, bool watchAd)
        {
            if (rewardList == null || rewardList.Count == 0)
                return;
            
            if (!stillClaimable)
                SetState(GetReceivedState());
            
            var tasks = new List<UniTask>();
            for (int i = 0, j = -1; i < rewardList.Count; i++)
            {
                if (j + 1 < rewardItems.Count)
                    j++;

                var spawnPosition = rewardParent?.position ?? Vector3.zero;
                if (0 <= j && j < rewardItems.Count)
                    spawnPosition = rewardItems[j].transform.position;

                var rewardInfo = rewardList[i];
                var spawnTask = DailyRewardsHelper.SpawnItemAsync(rewardInfo.Reward, rewardInfo.Amount, spawnPosition,
                    coinBarTransform.position, watchAd);
                tasks.Add(spawnTask);
            }
            
            await UniTask.WhenAll(tasks);
        }

        public override Vector3 GetCoinbarPosition() => coinBarTransform ? coinBarTransform.position : Vector3.zero;
    }
}

namespace DailyRewards
{
    public partial class SpecialRewardSlot
    {
        public abstract class SpecialAbstractState : AbstractItemState
        {
            protected readonly SpecialRewardSlot Context;

            public virtual void UpdateContent()
            {
                Context.dayText.text = $"Day {Context.dateData.Day}";

                var rewardList = Context.dateData.RewardList;
                for (var i = Context.rewardItems.Count; i < rewardList.Count; i++)
                    Context.rewardItems.Add(Instantiate(Context.rewardItemPrefab, Context.rewardParent));

                for (var i = 0; i < Context.rewardItems.Count; i++)
                {
                    if (i >= rewardList.Count)
                    {
                        Context.rewardItems[i].gameObject.SetActive(false);
                        continue;
                    }

                    var rewardData = Context.dateData.RewardList[i];
                    Context.rewardItems[i].UpdateDisplayWithRewardId(rewardData.Reward, rewardData.Amount).Forget();
                }
                
                Context.receivedFrontPanel.SetActive(false);
                Context.receivedBackPanel.SetActive(false);
                Context.receivablePanel.SetActive(false);
            }

            protected SpecialAbstractState(SpecialRewardSlot context)
            {
                Context = context;
            }
        }

        public class SpecialReceivableState : SpecialAbstractState
        {
            public SpecialReceivableState(SpecialRewardSlot context) : base(context)
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

        public class SpecialReceivedState : SpecialAbstractState
        {
            public SpecialReceivedState(SpecialRewardSlot context) : base(context)
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

        public class SpecialUnavailableState : SpecialAbstractState
        {
            public SpecialUnavailableState(SpecialRewardSlot context) : base(context)
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