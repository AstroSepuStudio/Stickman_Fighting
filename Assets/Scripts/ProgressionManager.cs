using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ProgressionManager : MonoBehaviour
{
    [SerializeField] bool _forceNewFile;
    public static PlayerData Player_Data;
    private static string FilePath { get => Path.Combine(Application.persistentDataPath, "playerData.json"); }

    [SerializeField] TextMeshProUGUI _killText;
    static UnityEvent _enemyKilled = new();

    private void Awake()
    {
        if (_forceNewFile)
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }

        Player_Data ??= LoadPlayerData();

        if (_killText == null) return;
        _enemyKilled.AddListener(UpdateText);
        _killText.SetText($"Kill Points: {Player_Data.Kills}");
    }

    private void OnApplicationQuit() { SaveData(); _enemyKilled.RemoveAllListeners(); }

    private void OnDestroy() { SaveData(); _enemyKilled.RemoveAllListeners(); }

    private void UpdateText() { _killText.SetText($"Kill Points: {Player_Data.Kills}"); }

    public static void EnemyKilled()
    {
        Player_Data ??= LoadPlayerData();

        Player_Data.Kills++;
        Player_Data.Gold += 5;
        _enemyKilled.Invoke();
    }

    public static void DamageIncrease()
    {
        Player_Data ??= LoadPlayerData();

        Player_Data.DamageIncreaseLevel += 1;

        SaveData();
    }

    public static void HealthIncrease()
    {

        Player_Data.HealthIncreaseLevel += 1;

        SaveData();
    }

    public static void SaveData()
    {
        Player_Data ??= LoadPlayerData();

        string json = JsonUtility.ToJson(Player_Data, true);
        File.WriteAllText(FilePath, json);
        Debug.Log("Saved to: " + FilePath);
    }

    public static bool ConsumeGold(int amount)
    {
        Player_Data ??= LoadPlayerData();

        if (Player_Data.Gold >= amount)
        {
            Player_Data.Gold -= amount;
            return true;
        }

        return false;
    }

    public static PlayerData LoadPlayerData()
    {
        if (!File.Exists(FilePath))
        {
            Debug.LogWarning("Save file not found. Returning new data.");
            return new PlayerData(0, 100);
        }

        string json = File.ReadAllText(FilePath);
        PlayerData data = JsonUtility.FromJson<PlayerData>(json);
        return data;
    }
}

[System.Serializable]
public class PlayerData
{
    public int Kills;
    public int Gold;
    public int DamageIncreaseLevel;
    public int HealthIncreaseLevel;
    public float Energy;
    public string LastTimeRecoveredEnergy;
    public string ClaimedRewardTime;

    public PlayerData(int killPoints, float energy)
    {
        Kills = killPoints;
        Energy = energy;
        Gold = killPoints * 5;
        DamageIncreaseLevel = 0;
        HealthIncreaseLevel = 0;
        LastTimeRecoveredEnergy = DateTime.MinValue.ToString("o");
        ClaimedRewardTime = DateTime.MinValue.ToString("o");
    }
}
