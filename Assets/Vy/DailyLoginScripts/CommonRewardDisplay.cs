using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

namespace DailyRewards
{
    public class CommonRewardDisplay : AbstractRewardDisplay
    {
        [SerializeField] protected Transform transformSpawningReward;
        [SerializeField] string rewardAmountFormat;
        [SerializeField] protected TextMeshProUGUI rewardAmountTMP;
        public override void Initialize(string rewardId, int rewardCount)
        {
            id = rewardId;
            UpdateRewardCountText(rewardCount);
        }

        public override void UpdateRewardCountText(int rewardCount)
        {
            count = rewardCount;
            if (rewardAmountTMP != null)
                rewardAmountTMP.text = string.Format(rewardAmountFormat, rewardCount);
        }

        public override Vector3 GetWorldPositionSpawningReward() => transformSpawningReward == null ? Vector3.zero : transformSpawningReward.position;
    }
}


