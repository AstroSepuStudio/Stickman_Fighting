using System.Collections.Generic;
using UnityEngine;

public class ShakeHandler : MonoBehaviour
{
    public static ShakeHandler Instance;

    [SerializeField] List<Rigidbody> _shakeTargets = new List<Rigidbody>();

    [SerializeField] float counterGravity = 0.1f;
    [SerializeField] float forceMultiplier = 5f;
    [SerializeField] int xMultiplier = 1;
    [SerializeField] int yMultiplier = 1;
    [SerializeField] int zMultiplier = 1;

    private void Awake()
    {
        Instance = this;
    }

    void FixedUpdate()
    {
        Vector3 accel = Input.acceleration;
        accel.x *= xMultiplier;
        accel.y *= yMultiplier;
        accel.z *= zMultiplier;

        Vector3 shakeForce = accel * forceMultiplier;

        foreach (Rigidbody rb in _shakeTargets)
        {
            if (rb != null)
            {
                rb.AddForce(shakeForce, ForceMode.Impulse);
                rb.AddForce(Vector3.up * counterGravity);
            }
        }
    }

    public void AddEnemyHipToShakeTargets(Rigidbody rigidbody)
    {
        if (!_shakeTargets.Contains(rigidbody))
            _shakeTargets.Add(rigidbody);
    }

    public void RemoveEnemyHipToShakeTargets(Rigidbody rigidbody)
    {
        if (_shakeTargets.Contains(rigidbody))
            _shakeTargets.Remove(rigidbody);
    }
}
