using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] string _sceneName;

    [SerializeField] TextMeshProUGUI _killPointsTxt;

    private void Start()
    {
        if (_killPointsTxt != null)
            _killPointsTxt.SetText(ProgressionManager.LoadPlayerData().killPoints.ToString());
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(_sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void CloseWindow(GameObject window)
    {
        window.SetActive(false);
    }

    public void OpenWindow(GameObject window)
    {
        window.SetActive(true);
    }

    public void IncreasePlayerDamage()
    {
        if (ProgressionManager.ConsumeKillPoint())
        {
            ProgressionManager.DamageIncrease();
            _killPointsTxt.SetText(ProgressionManager.Player_Data.killPoints.ToString());
            ProgressionManager.SaveData();
        }
    }

    public void IncreasePlayerHealth()
    {
        if (ProgressionManager.ConsumeKillPoint())
        {
            ProgressionManager.HealthIncrease();
            _killPointsTxt.SetText(ProgressionManager.Player_Data.killPoints.ToString());
            ProgressionManager.SaveData();
        }
    }
}
