using UnityEngine;

public class DamageBox : MonoBehaviour
{
    public Entity _parentEntity;

    [SerializeField] string _targetTag;

    private void Awake()
    {
        _parentEntity = GetComponentInParent<Entity>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(_targetTag))
        {
            BodyPart target = collision.gameObject.GetComponent<BodyPart>();

            if (target != null)
                target.TakeDamage(collision.relativeVelocity.magnitude);
        }
    }
}
