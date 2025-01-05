using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UserProfile", menuName = "UserProfile", order = 1)]
public class UserProfile : ScriptableObject
{
    [SerializeField] private ProfileData profile;
    public void SetProfile(ProfileData userProfile) => this.profile = userProfile;
    public ProfileData Profile => profile;

    public void ClaimCachedData()
    {
        //claim cached coin
        profile.ingameCurrency += profile.cachedInGameCurrency;
        profile.cachedInGameCurrency = 0;

        // claim cached booster
        for (int i = 0, length = profile.cachedbooster.Count; i < length; ++i)
        {
            int amount = profile.cachedbooster[i].amount;
            if (amount > 0)
                AddBoosterAmount(i, amount);
            profile.cachedbooster[i].amount = 0;
        }
    }
    
    public Action LevelChangedEvent;
    public int CurrentLevel
    {
        get => profile.currentLevel;
        set
        {
            if (profile.currentLevel != value)
            {
                profile.currentLevel = value;
                LevelChangedEvent?.Invoke();
                isDirty = true;
            }
        }
    }

    public int CurrentLoopLevel
    {
        get => profile.currentLoopLevel;
        set
        {
            if (profile.currentLoopLevel != value)
            {
                profile.currentLoopLevel = value;
                isDirty = true;
            }
        }
    }

    public int InGameCurrency
    {
        get => profile.ingameCurrency;
        set
        {
            if (profile.ingameCurrency != value)
            {
                OnCurrencyChanged?.Invoke(value, value > profile.ingameCurrency);
                profile.ingameCurrency = value;
                isDirty = true;
            }
        }
    }

    public int CachedIngameCurrency
    {
        get => profile.cachedInGameCurrency;
        set
        {
            if (profile.cachedInGameCurrency != value)
            {
                profile.cachedInGameCurrency = value;
                isDirty = true;
            }
        }
    }

    public bool IsOldUser
    {
        get => profile.isOldUser;
        set
        {
            if (profile.isOldUser != value)
            {
                profile.isOldUser = value;
                isDirty = true;
            }
        }
    }

    public bool IsBgEvent
    {
        get => profile.isBgEvent;
        set
        {
            if (profile.isBgEvent != value)
            {
                profile.isBgEvent = value;
                isDirty = true;
            }
        }
    }

    public int SavedQualityLevel
    {
        get => profile.savedQualityLevel;
        set
        {
            if (profile.savedQualityLevel != value)
            {
                profile.savedQualityLevel = value;
                isDirty = true;
            }
        }
    }

    public int TimeStampInSeconds_firstTimeOpen
    {
        get => profile.timeStampInSeconds_firstTimeOpen;
        set
        {
            if (profile.timeStampInSeconds_firstTimeOpen != value)
            {
                profile.timeStampInSeconds_firstTimeOpen = value;
                isDirty = true;
            }
        }
    }

    public int UndoBoosterQuantity
    {
        get => profile.undoBoosterQuantity;
        set
        {
            if (profile.undoBoosterQuantity != value)
            {
                profile.undoBoosterQuantity = value;
                isDirty = true;
            }
        }
    }

    public int MagnetBoosterQuantity
    {
        get => profile.magnetBoosterQuantity;
        set
        {
            if (profile.magnetBoosterQuantity != value)
            {
                profile.magnetBoosterQuantity = value;
                isDirty = true;
            }
        }
    }

    public int ShuffleBoosterQuantity
    {
        get => profile.shuffleBoosterQuantity;
        set
        {
            if (profile.shuffleBoosterQuantity != value)
            {
                profile.shuffleBoosterQuantity = value;
                isDirty = true;
            }
        }
    }

    public string EndlessLevel
    {
        get => profile.endlessLevel;
        set
        {
            if (profile.endlessLevel != value)
            {
                profile.endlessLevel = value;
                isDirty = true;
            }
        }
    }

    public int FreeCoinTurnClaim
    {
        get => profile.freeCoinTurnClaim;
        set
        {
            if (profile.freeCoinTurnClaim != value)
            {
                profile.freeCoinTurnClaim = value;
                isDirty = true;
            }
        }
    }

    private bool isDirty = false;
    public bool IsDirty => isDirty;
    public bool MarkDirty() => isDirty = true;
    public void CleanDirty() => isDirty = false;
    public Action<int, bool> OnCurrencyChanged;

    public void AddBoosterAmount(int boosterID, int value)
    {
        var currentAmount = GetBoosterData(boosterID).amount;
        UpDataBoosterAmount(boosterID, currentAmount + value);
    }

    public BoosterData GetBoosterData(int boosterTye)
    {
        if (boosterTye < profile.boosterDatas.Count) return profile.boosterDatas[boosterTye];
        else
        {
            BoosterData data = new();
            profile.boosterDatas.Add(data);
            isDirty = true;
            return data;
        }
    }

    public BoosterData GetCachedBoosterData(int boosterID)
    {
        if (profile.cachedbooster == null) profile.cachedbooster = new List<BoosterData>();

        if (boosterID >= profile.cachedbooster.Count)
        {
            for (int i = profile.cachedbooster.Count; i <= boosterID; ++i)
                profile.cachedbooster.Add(new BoosterData());
        }

        return profile.cachedbooster[boosterID];
    }

    public void AddCachedBooster(int boosterID, int value)
    {
        var data = GetCachedBoosterData(boosterID);
        if (data != null)
        {
            data.amount += value;
            isDirty = true;
        }
    }

    public void DecreaseCachedBooster(int boosterID, int value)
    {
        var data = GetCachedBoosterData(boosterID);
        if (data != null)
        {
            data.amount -= value;
            if (data.amount < 0) data.amount = 0;
            isDirty = true;
        }
    }

    public void UpDataBoosterAmount(int boosterType, int value)
    {
        if (value < 0) value = 0;
        var data = GetBoosterData(boosterType);
        int lastAmount = data.amount;
        data.amount = value;
        isDirty = true;
        BoosterAmountChanged?.Invoke(boosterType, value, lastAmount < value);
    }

    public Action<int, int, bool> BoosterAmountChanged;


    public DailyRewardsUserData DailyRewardsUserData
    {
        get
        {
            if (profile.dailyRewardsData == null)
            {
                profile.dailyRewardsData = new DailyRewardsUserData();
                isDirty = true;
            }
            
            return profile.dailyRewardsData;
        }
    }
}

[Serializable]
public class ProfileData
{
    public int currentLevel;
    public int currentLoopLevel;
    public int ingameCurrency;
    public int cachedInGameCurrency = 0;
    public bool isOldUser;
    public int savedQualityLevel;
    public int timeStampInSeconds_firstTimeOpen;
    public int undoBoosterQuantity;
    public int magnetBoosterQuantity;
    public int shuffleBoosterQuantity;
    public string endlessLevel;
    public int freeCoinTurnClaim;
    public bool isBgEvent;
    public List<BoosterData> boosterDatas = new();
    public List<BoosterData> cachedbooster = new();
    public DailyRewardsUserData dailyRewardsData;

    public ProfileData()
    {
        timeStampInSeconds_firstTimeOpen = 0;
        ingameCurrency = 0;
        isOldUser = false;
        savedQualityLevel = -1;
        currentLevel = 0;
        currentLoopLevel = 0;
        undoBoosterQuantity = 4;
        magnetBoosterQuantity = 4;
        shuffleBoosterQuantity = 4;
        endlessLevel = string.Empty;
        freeCoinTurnClaim = 0;
        isBgEvent = false;
        boosterDatas = cachedbooster = new List<BoosterData>();
    }
}

[Serializable]
public class BoosterData
{
    public int amount = 0;
}

public enum BoosterType
{
    None = -1,
    Refresh,
    Clear,
    Sort
}