using System;
using UnityEngine;
using UnityEngine.Android;
using Unity.Notifications.Android;

public class RewardManager : MonoBehaviour
{
    private DateTime _time4NxtReward;

    private void Start()
    {
        // Get the DateTime of the nextrewardtime

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
    }

    public void ClaimDailyReward()
    {
        DateTime currentTime = TimeZone.CurrentTime;
        if (currentTime < _time4NxtReward)
        {
            Debug.Log("Todavia le falta manito");
            return;
        }

        _time4NxtReward = currentTime.AddDays(1);
        Debug.Log($"Next reward: {_time4NxtReward}");

        // Add 100 energy to player
        // Add gold to player

        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
        AndroidNotificationCenter.CancelAllDisplayedNotifications();

        //Create the Notification
        var notification = new AndroidNotification();
        notification.Title = "Oiga mijo, venga pa' aca";
        notification.Text = "¡Reclama tu recompensa diaria!";
        notification.FireTime = currentTime.AddSeconds(15);
        Debug.Log($"Set notification at {currentTime.AddSeconds(15)}");
        //Send the notification
        var id = AndroidNotificationCenter.SendNotification(notification, "channel_id");

        //if the script is run and a message is already scheduled, cancel it and re-schedule another message
        if (AndroidNotificationCenter.CheckScheduledNotificationStatus(id) == NotificationStatus.Scheduled)
        {
            AndroidNotificationCenter.CancelAllNotifications();
            AndroidNotificationCenter.SendNotification(notification, "channel_id");
        }
    }
}
