using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.Events;

public class TimeZone : MonoBehaviour
{
    public static DateTime AppOpenTime { get; private set; }
    public static DateTime CurrentTime => AppOpenTime.AddSeconds(Time.time);
    public static UnityEvent OnGetDate = new();
    public static bool GotDate = false;

    [TextArea(1, 3)]
    [SerializeField] string URL = "http://worldtimeapi.org/api/timezone/America/Santiago";

    private void Awake()
    {
        if (AppOpenTime != DateTime.MinValue) return;

        StartCoroutine(GetData());
    }

    IEnumerator GetData()
    {
        int retryCount = 0;
        int maxRetries = 5;

        while (AppOpenTime == DateTime.MinValue && retryCount < maxRetries)
        {
            Debug.Log("Try get time");

            using UnityWebRequest request = UnityWebRequest.Get(URL);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error fetching time: {request.error}");
            }
            else
            {
                string json = request.downloadHandler.text;

                try
                {
                    var stats = JSON.Parse(json);
                    string datetimeStr = stats["datetime"];

                    AppOpenTime = DateTime.Parse(datetimeStr, null, System.Globalization.DateTimeStyles.RoundtripKind);
                    Debug.Log($"Time when the app opened: {AppOpenTime}");
                    OnGetDate?.Invoke();
                    GotDate = true;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Parsing error: {ex.Message}");
                }
            }

            retryCount++;
            yield return new WaitForSeconds(1);
        }

        if (AppOpenTime == DateTime.Now)
        {
            Debug.LogError("Failed to fetch time after multiple attempts.");
        }
    }
}
