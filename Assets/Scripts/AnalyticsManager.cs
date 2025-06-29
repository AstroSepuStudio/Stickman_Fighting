using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance;

    private async void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
            return;
        }

        Destroy(gameObject);
    }

    public void OnRunCompleted(int currentRound)
    {
        int completedRounds = currentRound--;
        int upgradesBought = ProgressionManager.GetTotalUpgrades();

        CustomEvent customEvent = new
        ("completedRun")
        {
            { "Rounds_Complete", completedRounds },
            { "Upgrades_Bought", upgradesBought }
        };

        AnalyticsService.Instance.RecordEvent(customEvent);
        Debug.Log("Custom Analytics Event Sent: completedRun");
    }
}
