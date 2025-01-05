using System;
using UnityEngine.Serialization;

[Serializable]
public class DailyRewardsUserData
{
    public int currentDay = 0;
    public bool claimed;
    public long rewardDateTime;
}
