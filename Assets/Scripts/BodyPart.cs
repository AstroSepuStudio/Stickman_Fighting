using UnityEngine;

public class BodyPart : MonoBehaviour
{
    Entity _parentEntity;

    [SerializeField] float _damageMultiplier = 1;

    private void Awake()
    {
        _parentEntity = GetComponentInParent<Entity>();
    }

    public void TakeDamage(float amount)
    {
        _parentEntity.TakeDamage(amount * _damageMultiplier);
    }
}
