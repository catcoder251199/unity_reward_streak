using System;

namespace DailyRewards
{
    [Serializable]
    public struct RewardItemData
    {
        public string Reward;
        public int Amount;

        public RewardItemData(string reward, int amount)
        {
            Reward = reward;
            Amount = amount;
        }
    }
}