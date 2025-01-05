using UnityEngine;

namespace RewardStreak
{
    public abstract class AbstractRewardStreakStorage : MonoBehaviour
    {
        [SerializeField] protected RewardStreakUserData userData;
    
        public virtual RewardStreakUserData UserData => userData;
        public abstract int CurrentLevel { get; }
        public abstract void Initialize();
        public abstract void MarkDirty();
    }
}