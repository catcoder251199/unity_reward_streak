namespace RewardStreak
{
    public static class RewardStreakConstants
    {
        public static readonly string RewardStreakAdsLocation = "daily_rewards_ads";
        public static readonly string RewardStreakMinLevelKey = "RewardStreakMinLevel";
        public static int RewardStreakMinLevelDefault => 5;
        public static int FreeRewardCooldownDefault => 3600; // seconds
        public static int AdsRewardCooldownDefault => 43200; // seconds
        public static int ButtonClickDelay => 150; // milliseconds

        #region Reward Id

        public static string Gold => "Gold";
        public static string Refresh => "Refresh";
        public static string Clear => "Clear";
        public static string Sort => "Sort";

        #endregion
    }
}