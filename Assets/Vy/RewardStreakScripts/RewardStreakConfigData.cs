using System;
using System.Collections.Generic;
using UnityEngine;

namespace RewardStreak
{
    [Serializable]
    public class Reward
    {
        [SerializeField] private string id;
        [SerializeField] private int amount;
        
        public string Id => id;
        public int Amount => amount;
    }
    
    [Serializable]
    public class RewardStreakConfigData
    {
        [SerializeField] private int freeRewardCooldown = RewardStreakConstants.FreeRewardCooldownDefault;
        [SerializeField] private int adsRewardCooldown = RewardStreakConstants.AdsRewardCooldownDefault;
        [SerializeField] private List<Reward> freeRewards;
        [SerializeField] private List<Reward> adsRewards;
        
        public readonly List<Reward> FreeRewards = new List<Reward>();
        public readonly List<Reward> AdsRewards = new List<Reward>();
        public int FreeRewardCooldown => freeRewardCooldown;
        public int AdsRewardCooldown => adsRewardCooldown;
        
        public string ConvertDataToString()
        {
            var ret = JsonUtility.ToJson(this);
            return ret;
        }
    }
}