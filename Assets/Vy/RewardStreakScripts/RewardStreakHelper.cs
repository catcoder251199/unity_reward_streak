using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RewardStreak
{
    public static class RewardStreakHelper
    {
        public static DateTime GetCurrentTimeUtc() => DateTime.UtcNow;
        public static long GetCurrentTimeUtcInSeconds() => new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
    }
}

