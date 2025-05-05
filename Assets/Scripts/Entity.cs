using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Entity : MonoBehaviour
{
    [SerializeField] bool _isPlayer;
    [SerializeField] Rigidbody _hipRigidbody;
    [SerializeField] Canvas _enemyCanvas;
    [SerializeField] Image _hpBar;
    public float _maxHP;
    [SerializeField] float _currentHP;

    Vector3 offset = Vector3.zero;

    public UnityEvent<Entity> OnDeath;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (_enemyCanvas != null)
            offset = _enemyCanvas.transform.position - _hipRigidbody.transform.position;
        ShakeHandler.Instance.AddEnemyHipToShakeTargets(_hipRigidbody);

        if (_isPlayer)
            _maxHP *= ProgressionManager.Player_Data.HealthIncrease;

        _currentHP = _maxHP;
    }

    private void Update()
    {
        if (offset != Vector3.zero)
        {
            _enemyCanvas.transform.position = offset + _hipRigidbody.transform.position;
        }
    }

    public void TakeDamage(float amount)
    {
        if (!_isPlayer)
            amount *= ProgressionManager.Player_Data.DamageIncrease;

        _currentHP -= amount;
        _hpBar.fillAmount = _currentHP / _maxHP;

        if (_currentHP <= 0)
        {
            if (!_isPlayer)
                ProgressionManager.EnemyKilled();

            OnDeath?.Invoke(this);

            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        ShakeHandler.Instance.RemoveEnemyHipToShakeTargets(_hipRigidbody);
    }

    private void OnDestroy()
    {
        ShakeHandler.Instance.RemoveEnemyHipToShakeTargets(_hipRigidbody);
    }
}
