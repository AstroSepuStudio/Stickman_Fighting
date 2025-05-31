using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class TimeZone : MonoBehaviour
{
    public static DateTime AppOpenTime { get; private set; }
    public static DateTime CurrentTime => AppOpenTime.AddSeconds(Time.time);

    [SerializeField] string URL = "http://worldtimeapi.org/api/timezone/America/Santiago";

    private void Awake()
    {
        if (AppOpenTime != DateTime.MinValue) return;

        StartCoroutine(GetData());
    }

    IEnumerator GetData()
    {
        while (AppOpenTime == DateTime.MinValue)
        {
            Debug.Log("Try get time");
            using UnityWebRequest request = UnityWebRequest.Get(URL);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
                Debug.LogError(request.error);
            else
            {
                string json = request.downloadHandler.text;
                JSONNode stats = JSON.Parse(json);

                AppOpenTime = DateTime.Parse(stats["datetime"]);
                Debug.Log($"Time when the app oppened: {AppOpenTime}");
            }
        }
    }
}
