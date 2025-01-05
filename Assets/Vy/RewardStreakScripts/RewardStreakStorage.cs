using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RewardStreak
{
    public class RewardStreakStorage : AbstractRewardStreakStorage
    {
        [SerializeField] private UserProfile userProfile;
      
        public override void Initialize()
        {
            LoadDataFromStorage();
        }

        private void LoadDataFromStorage()
        {
            if (userProfile == null)
                userProfile = TryToFindUserProfile();
            if (userProfile != null)
                userData = userProfile.RewardStreakUserData;
        }

        private UserProfile TryToFindUserProfile()
        {
            if (GameMaster.Instance == null)
                return null;
            
            return GameMaster.Instance.GetUserProfile();
        }
    
        public override int CurrentLevel => userProfile != null ? userProfile.CurrentLevel : 0;

        public override void MarkDirty()
        {
            if (userProfile != null)
                userProfile.MarkDirty();
        }
    } 
}