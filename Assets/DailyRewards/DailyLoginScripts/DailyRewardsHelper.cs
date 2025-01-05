using System;
using Cysharp.Threading.Tasks;
//using TrackingHelper;
using UnityEngine;

public static class DailyRewardsHelper
{
    public static DateTime GetCurrentLocalDateTime() => DateTime.Now;

    public static DateTime ConvertLongToDateTime(long value) =>
        DateTimeOffset.FromUnixTimeSeconds(value).ToLocalTime().DateTime;

    public static long ConvertDateTimeToLong(DateTime date)
    {
        var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        var diff = date.ToUniversalTime() - origin;
        return (long)Math.Floor(diff.TotalSeconds);
    }
    
    public static long GetStartOfTodayAsLong()
    {
        return new DateTimeOffset(DateTime.Today).ToUnixTimeSeconds();
    }
    
    public static long GetStartTomorrowAsLong()
    {
        return new DateTimeOffset(DateTime.Today.AddDays(1)).ToUnixTimeSeconds();
    }
    
    public static void SpawnItem(string rewardId, int rewardAmount, Vector3 spawnPosition, Vector3 destinationPosition, bool watchAd)
    {
        Debug.Log($"Spawn Item {rewardId}: {rewardAmount}");

        var userProfile = GameMaster.Instance.GetUserProfile();        
        switch (rewardId)
        {
            case DailyRewardsConstants.Gold:
                userProfile.InGameCurrency += rewardAmount;
                break;
            case DailyRewardsConstants.Refresh:
                userProfile.AddBoosterAmount((int) BoosterType.Refresh, rewardAmount);
                break;
            case DailyRewardsConstants.Clear:
                userProfile.AddBoosterAmount((int) BoosterType.Clear, rewardAmount);
                break;
            case DailyRewardsConstants.Sort:
                userProfile.AddBoosterAmount((int) BoosterType.Sort, rewardAmount);
                break;
        }
    }
    
    public static async UniTask SpawnItemAsync(string rewardId, int rewardAmount, Vector3 spawnPosition, Vector3 destinationPosition, bool watchAd)
    {
        var userProfile = GameMaster.Instance.GetUserProfile();        
        switch (rewardId)
        {
            case DailyRewardsConstants.Gold:
                userProfile.InGameCurrency += rewardAmount;
                await UniTask.Delay(TimeSpan.FromSeconds(2));
                break;
            case DailyRewardsConstants.Refresh:
                userProfile.AddBoosterAmount((int) BoosterType.Refresh, rewardAmount);
                await UniTask.Delay(TimeSpan.FromSeconds(1));
                break;
            case DailyRewardsConstants.Clear:
                userProfile.AddBoosterAmount((int) BoosterType.Clear, rewardAmount);
                await UniTask.Delay(TimeSpan.FromSeconds(1));
                break;
            case DailyRewardsConstants.Sort:
                userProfile.AddBoosterAmount((int) BoosterType.Sort, rewardAmount);
                await UniTask.Delay(TimeSpan.FromSeconds(1));
                break;
        }
        Debug.Log($"Spawned Item Async {rewardId}: {rewardAmount}");
    }
}
