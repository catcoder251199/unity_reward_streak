using DailyRewards;
using UnityEngine;
using Reward = RewardStreak.Reward;

namespace RewardStreak
{
    public class RewardStreakItem : MonoBehaviour
    {
        [SerializeField] private AbstractRewardStreakButton claimButton;
        [SerializeField] private RewardStreakRewardItem rewardItem;
        [SerializeField] private int itemIndex = -1;
        public void Initialize(int index, Reward reward)
        {
            itemIndex = itemIndex;
            rewardItem.UpdateDisplayWithRewardId(reward.Id, reward.Amount).Forget();
            claimButton.Initialize(reward.Id, reward.Amount);
        }
    }
}