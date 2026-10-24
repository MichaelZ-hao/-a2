using UnityEngine;

[DisallowMultipleComponent]
/// <summary>Moves a platform at a fixed speed between two assigned end points.</summary>
public sealed class MovingPlatform : MonoBehaviour
{
    [Header("Route")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [Min(0.01f)] [SerializeField] private float speed = 2f;
    [SerializeField] private bool startTowardsPointB = true;
    [SerializeField] private bool carryPlayer = true;

    private Transform currentTarget;
    private BoxCollider platformCollider;

    private void Awake()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError("MovingPlatform requires Point A and Point B.", this);
            enabled = false;
            return;
        }

        currentTarget = startTowardsPointB ? pointB : pointA;
        platformCollider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        Vector3 previousPosition = transform.position;
        transform.position = Vector3.MoveTowards(
            transform.position,
            currentTarget.position,
            speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, currentTarget.position) < 0.001f)
        {
            currentTarget = currentTarget == pointA ? pointB : pointA;
        }

        if (carryPlayer)
        {
            MovePassengers(transform.position - previousPosition);
        }
    }

    private void MovePassengers(Vector3 movement)
    {
        if (platformCollider == null || movement.sqrMagnitude <= 0f)
        {
            return;
        }

        Vector3 scale = transform.lossyScale;
        Vector3 localSize = platformCollider.size;
        float platformTop = platformCollider.bounds.max.y;
        Vector3 center = new Vector3(
            platformCollider.bounds.center.x,
            platformTop + 0.12f,
            platformCollider.bounds.center.z);
        Vector3 halfExtents = new Vector3(
            localSize.x * scale.x * 0.45f,
            0.16f,
            localSize.z * scale.z * 0.45f);

        foreach (Collider hit in Physics.OverlapBox(center, halfExtents, transform.rotation))
        {
            CharacterController controller = hit.GetComponentInParent<CharacterController>();
            if (controller != null && controller.enabled &&
                controller.bounds.min.y >= platformTop - 0.08f)
            {
                controller.Move(movement);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (pointA == null || pointB == null)
        {
            return;
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pointA.position, pointB.position);
        Gizmos.DrawWireSphere(pointA.position, 0.2f);
        Gizmos.DrawWireSphere(pointB.position, 0.2f);
    }
}
