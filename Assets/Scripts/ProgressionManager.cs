using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ProgressionManager : MonoBehaviour
{
    public static PlayerData Player_Data;
    private static string FilePath { get => Path.Combine(Application.persistentDataPath, "playerData.json"); }

    [SerializeField] TextMeshProUGUI _killText;
    static UnityEvent _enemyKilled = new();

    private void Awake()
    {
        if (_killText == null) return;
        Player_Data ??= LoadPlayerData();

        _enemyKilled.AddListener(UpdateText);
        _killText.SetText($"Kill Points: {Player_Data.killPoints}");
    }

    private void OnApplicationQuit() { SaveData(); _enemyKilled.RemoveAllListeners(); }

    private void OnDestroy() { SaveData(); _enemyKilled.RemoveAllListeners(); }

    private void UpdateText() { _killText.SetText($"Kill Points: {Player_Data.killPoints}"); }

    public static void EnemyKilled()
    {
        Player_Data ??= LoadPlayerData();

        Player_Data.killPoints++;
        _enemyKilled.Invoke();
    }

    public static void DamageIncrease()
    {
        Player_Data ??= LoadPlayerData();

        Player_Data.DamageIncrease += 0.1f;

        SaveData();
    }

    public static void HealthIncrease()
    {

        Player_Data.HealthIncrease += 0.1f;

        SaveData();
    }

    public static void SaveData()
    {
        Player_Data ??= LoadPlayerData();

        string json = JsonUtility.ToJson(Player_Data, true);
        File.WriteAllText(FilePath, json);
        Debug.Log("Saved to: " + FilePath);
    }

    public static bool ConsumeKillPoint()
    {
        Player_Data ??= LoadPlayerData();

        if (Player_Data.killPoints > 0)
        {
            Player_Data.killPoints--;
            return true;
        }

        return false;
    }

    public static PlayerData LoadPlayerData()
    {
        if (!File.Exists(FilePath))
        {
            Debug.LogWarning("Save file not found. Returning new data.");
            return new PlayerData(0);
        }

        string json = File.ReadAllText(FilePath);
        PlayerData data = JsonUtility.FromJson<PlayerData>(json);
        return data;
    }
}

[System.Serializable]
public class PlayerData
{
    public int killPoints;
    public float DamageIncrease;
    public float HealthIncrease;

    public PlayerData(int killPoints) 
    { 
        this.killPoints = killPoints;
        DamageIncrease = 1;
        HealthIncrease = 1;
    }
}
