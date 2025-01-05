using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DailyRewards;

public class HomeController : MonoBehaviour
{
    [SerializeField] private DailyRewardsButton _dailyRewardsButton;
    [SerializeField] private Button[] buttons;

    private static bool _firstEnterHome = true;
    private static int _lastLevel = 0;
    public static bool HasUserJustCompletedLevel => !_firstEnterHome && GameMaster.Instance.DisplayLevel > _lastLevel;
    
    private async void Start()
    {
        OnHomeEnteredBegin();
        try
        {
            if (_dailyRewardsButton != null) 
                await _dailyRewardsButton.OnHomeEnter();
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.25));
        }
        catch (Exception e)
        {
            Debug.LogError($"{nameof(HomeController)}" + e);
        }
        
        OnHomeEnterEnd();
        
        if (_firstEnterHome)
            _firstEnterHome = false;
        _lastLevel = GameMaster.Instance.DisplayLevel;
    }

    private void OnHomeEnteredBegin()
    {
        DisableAllHomeButtons();
    }

    private void EnableAllHomeButtons()
    {
        foreach (var button in buttons)
        {
            if (button == null) continue;
            button.interactable = true;
        }
    }

    private void DisableAllHomeButtons()
    {
        foreach (var button in buttons)
        {
            if (button == null) continue;
            button.interactable = false;
        }
    }

    private void OnHomeEnterEnd()
    {
        EnableAllHomeButtons();
        Debug.Log("Vy: Home Controller end");
    }

    [ContextMenu("Enter Home")]
    public void TryEnterHome()
    {
        Start();
    }
}