using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] string _gameSceneName;
    [SerializeField] string _sceneName;

    [SerializeField] float _beginGameCost;
    [SerializeField] TextMeshProUGUI _goldTxt;

    private void Start()
    {
        if (_goldTxt != null)
            _goldTxt.SetText(ProgressionManager.Player_Data.Gold.ToString());
    }

    public void TryStartGame()
    {
        if (ProgressionManager.Player_Data.Energy < _beginGameCost) return;

        ProgressionManager.Player_Data.Energy -= _beginGameCost;
        SceneManager.LoadScene(_gameSceneName);
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
        if (ProgressionManager.ConsumeGold(ProgressionManager.Player_Data.DamageIncreaseLevel * 5))
        {
            ProgressionManager.DamageIncrease();
            _goldTxt.SetText($"Gold: {ProgressionManager.Player_Data.Gold.ToString()}");
            ProgressionManager.SaveData();
        }
    }

    public void IncreasePlayerHealth()
    {
        if (ProgressionManager.ConsumeGold(ProgressionManager.Player_Data.HealthIncreaseLevel * 5))
        {
            ProgressionManager.HealthIncrease();
            _goldTxt.SetText($"Gold: {ProgressionManager.Player_Data.Gold.ToString()}");
            ProgressionManager.SaveData();
        }
    }

    public void RefreshGoldTxt()
    {
        if (_goldTxt != null)
            _goldTxt.SetText($"Gold: {ProgressionManager.Player_Data.Gold.ToString()}");
    }
}
