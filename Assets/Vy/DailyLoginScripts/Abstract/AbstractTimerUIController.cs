using UnityEngine;

namespace DailyRewards
{
    public abstract class AbstractTimerUIController : MonoBehaviour
    {
        public abstract void Initialize(DailyRewardsSystem dailyRewardsSystem);
        public abstract void UpdateTimer();
    }  
}