using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] Entity _playerEntity;
    [SerializeField] GameObject _enemyPrefab;
    [SerializeField] Transform[] _spawnArea;
    [SerializeField] TextMeshProUGUI _timerTxt;
    [SerializeField] TextMeshProUGUI _roundTxt;

    int _round;
    [SerializeField] List<Entity> _spawnEnemies = new();
    WaitForSeconds _sleep = new(1);

    [SerializeField] GameObject _gameOverScreen;

    private void Start()
    {
        _round = 1;
        SpawnEnemies();
        _roundTxt.SetText($"Round {_round}");
        _timerTxt.SetText("");

        _playerEntity.OnDeath.AddListener(GameOver);
        _gameOverScreen.SetActive(false);
    }

    void GameOver(Entity entity)
    {
        _gameOverScreen.SetActive(true);
        ProgressionManager.SaveData();
    }

    public void OnEnemyDeath(Entity entity)
    {
        _spawnEnemies.Remove(entity);
    }

    public void SpawnEnemies()
    {
        for (int i = 0; i < _round; i++)
        {
            GameObject enemy = Instantiate(_enemyPrefab);
            Entity enemyEntity = enemy.GetComponent<Entity>();
            _spawnEnemies.Add(enemyEntity);
            enemyEntity.OnDeath.AddListener(OnEnemyDeath);

            enemyEntity._maxHP *= 1 + (_round - 1) / 10;
            float randomX = Random.Range(_spawnArea[0].position.x, _spawnArea[1].position.x);
            float randomY = Random.Range(_spawnArea[0].position.y, _spawnArea[1].position.y);
            Vector2 RandomPosition = new(randomX, randomY);
            enemy.transform.position = RandomPosition;
        }

        StartCoroutine(WaitForNextWave());
    }

    IEnumerator WaitForNextWave()
    {
        while (_spawnEnemies.Count > 0)
        {
            Debug.Log("Waiting");
            yield return _sleep;
        }

        _round++;
        _roundTxt.SetText($"Round {_round}");

        float timer = 3;
        _timerTxt.SetText(3.ToString());
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            _timerTxt.SetText(Mathf.Round(timer).ToString());
            yield return null;
        }

        _timerTxt.SetText("");
        SpawnEnemies();
    }
}
