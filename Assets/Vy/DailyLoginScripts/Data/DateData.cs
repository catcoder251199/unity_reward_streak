using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DailyRewards
{
    [Serializable]
    public class DateData
    {
        [SerializeField] private int day;
        [SerializeField] private List<RewardItemData> rewardList; // List of (reward, amount)

        public DateData(int day, List<RewardItemData> rewardList)
        {
            this.day = day;
            this.rewardList = rewardList;
        }

        public int Day => day;
        public List<RewardItemData> RewardList => rewardList;
    }
}