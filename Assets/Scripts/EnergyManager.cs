using System;
using System.Collections;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.Android;

public class EnergyManager : MonoBehaviour
{
    [SerializeField] float _maxEnergy = 100f;
    [SerializeField] float _currentEnergy;

    [SerializeField] float _energyRecoverPerMinute = 0.5f;
    DateTime _lastTimeRecoveredEnergy;
    WaitForSeconds _waitMinute = new (60);

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

        StartCoroutine(RecoverEnergy());
    }

    IEnumerator RecoverEnergy()
    {
        while (TimeZone.AppOpenTime == DateTime.MinValue)
        {
            yield return null;
        }

        TimeSpan timeSpan = TimeZone.CurrentTime.Subtract(_lastTimeRecoveredEnergy);
        float totalEnergyRecover = timeSpan.Minutes * _energyRecoverPerMinute;
        _currentEnergy += totalEnergyRecover;
        if (_currentEnergy > _maxEnergy)
            _currentEnergy = _maxEnergy;

        float timer = 0;
        float waitTime = ((float)timeSpan.TotalMinutes - timeSpan.Minutes * 60) * 60;
        while (true)
        {
            if (_currentEnergy > _maxEnergy)
            {
                timer = 0;
                continue;
            }

            timer += Time.deltaTime;
            if (timer < waitTime) continue;
            timer -= waitTime;

            _lastTimeRecoveredEnergy = TimeZone.CurrentTime;

            _currentEnergy += _energyRecoverPerMinute;

            if (_currentEnergy > _maxEnergy)
                _currentEnergy = _maxEnergy;
        }
    }

    public bool HasEnoughEnergy(float cost)
    {
        if (_currentEnergy >= cost)
        {
            _currentEnergy -= cost;
            return true;
        }
        return false;
    }

    private void OnDestroy()
    {
        // Save to json last time the player recovered energy
    }
}
