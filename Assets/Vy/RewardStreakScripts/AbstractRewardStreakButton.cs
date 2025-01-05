using UnityEngine;

public abstract class AbstractRewardStreakButton : MonoBehaviour
{
    public abstract void Initialize(string rewardId, int rewardAmount);
    //public abstract void Claim();
    //public abstract void SetIsRewardedAdsType(bool isRewardedAdsType);
    //public abstract string ItemName { get; }
}
