using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DailyRewards
{
    public abstract class AbstractRewardDisplay : MonoBehaviour
    {
        [SerializeField] protected string id;
        [SerializeField] protected int count;
        public string RewardId  => id;
        public abstract void Initialize(string rewardId, int rewardCount);
        public virtual void UnInitialize(string rewardId, int rewardCount) {}
        public abstract void UpdateRewardCountText(int rewardCount);
        public abstract Vector3 GetWorldPositionSpawningReward();
    }
}