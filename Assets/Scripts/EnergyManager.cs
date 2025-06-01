using System;
using TMPro;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.Android;

public class EnergyManager : MonoBehaviour
{
    [SerializeField] float _maxEnergy = 100f;
    [SerializeField] float _energyRecoverPerMinute = 0.5f;
    [SerializeField] TextMeshProUGUI _energyTxt;

    float _timer;
    float _waitTime;

    private void Start()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
        AndroidNotificationCenter.CancelAllDisplayedNotifications();

        //Create the Android Notification Channel to send messages through
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Notifications Channel",
            Importance = Importance.Default,
            Description = "Reminder notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        if (_energyTxt != null)
            _energyTxt.SetText($"E: {ProgressionManager.Player_Data.Energy.ToString()}");

        TimeZone.OnGetDate.AddListener(BeginEnergyRecovery);
    }

    public void RefreshEnergyTxt()
    {
        if (_energyTxt != null)
            _energyTxt.SetText($"E: {ProgressionManager.Player_Data.Energy.ToString()}");
    }

    void BeginEnergyRecovery()
    {
        TimeSpan timeSpan = TimeZone.CurrentTime.Subtract(
            DateTime.Parse(ProgressionManager.Player_Data.LastTimeRecoveredEnergy, null, System.Globalization.DateTimeStyles.RoundtripKind));

        float totalEnergyRecover = Mathf.Round((float)timeSpan.TotalMinutes) * _energyRecoverPerMinute;
        ProgressionManager.Player_Data.Energy += totalEnergyRecover;
        if (ProgressionManager.Player_Data.Energy > _maxEnergy)
            ProgressionManager.Player_Data.Energy = _maxEnergy;

        Debug.Log($"Recovers {totalEnergyRecover} (minutes sinces last: {(int)timeSpan.TotalMinutes})");
        _energyTxt.SetText(ProgressionManager.Player_Data.Energy.ToString());

        _timer = 0;
        _waitTime = ((float)timeSpan.TotalMinutes - (int)timeSpan.TotalMinutes) * 60;
        Debug.Log($"Next recovery in {_waitTime} seconds (Total Minutes: {(float)timeSpan.TotalMinutes}, Minutes: {(int)timeSpan.TotalMinutes}, Subs: {(float)timeSpan.TotalMinutes - timeSpan.Minutes})");
        ProgressionManager.Player_Data.LastTimeRecoveredEnergy = TimeZone.CurrentTime.ToString("o");
    }

    private void Update()
    {
        if (!TimeZone.GotDate) return;

        if (ProgressionManager.Player_Data.Energy >= _maxEnergy)
        {
            _timer = 0;
            return;
        }

        _timer += Time.deltaTime;
        if (_timer < _waitTime) return;

        _timer = 0;
        _waitTime = 60;

        ProgressionManager.Player_Data.LastTimeRecoveredEnergy = TimeZone.CurrentTime.ToString("o");

        ProgressionManager.Player_Data.Energy += _energyRecoverPerMinute;

        if (ProgressionManager.Player_Data.Energy > _maxEnergy)
            ProgressionManager.Player_Data.Energy = _maxEnergy;

        _energyTxt.SetText(ProgressionManager.Player_Data.Energy.ToString());
    }
}
