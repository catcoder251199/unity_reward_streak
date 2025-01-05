using System;

namespace RewardStreak
{
    [Serializable]
    public class RewardStreakUserData
    {
        public long lastClaimedFreeTime;
        public long lastClaimedAdsTime;
        public int freeRewardStreak = 0;
        public int adsRewardsStreak = 0;
    }
}