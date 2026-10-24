using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
/// <summary>Rotates around a chosen local axis and reports controlled player hits.</summary>
public sealed class RotatingObstacle : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private Vector3 localRotationAxis = Vector3.up;
    [Min(0f)] [SerializeField] private float degreesPerSecond = 120f;

    [Header("Player Hit")]
    [SerializeField] private DeliveryMissionController mission;
    [Min(0f)] [SerializeField] private float timePenalty = 5f;
    [Min(0f)] [SerializeField] private float hitCooldown = 1.5f;
    [SerializeField] private UnityEvent onPlayerHit;

    private float nextAllowedHitTime;

    private void Update()
    {
        transform.Rotate(localRotationAxis.normalized, degreesPerSecond * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time < nextAllowedHitTime)
        {
            return;
        }

        if (other.GetComponentInParent<PlayerMovementController>() == null)
        {
            return;
        }

        nextAllowedHitTime = Time.time + hitCooldown;
        mission?.ApplyTimePenalty(timePenalty);
        onPlayerHit?.Invoke();
        Debug.Log("Player hit rotating obstacle.", this);
    }
}
