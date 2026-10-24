using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider))]
/// <summary>Updates the player's respawn point when entered.</summary>
public sealed class DeliveryCheckpoint : MonoBehaviour
{
    [Tooltip("If unassigned, this object's position and rotation are used as the respawn point.")]
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private bool activateOnlyOnce = true;

    private bool hasActivated;

    private void Reset()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activateOnlyOnce && hasActivated)
        {
            return;
        }

        PlayerRespawnController playerRespawn =
            other.GetComponentInParent<PlayerRespawnController>();

        if (playerRespawn == null)
        {
            return;
        }

        playerRespawn.SetCheckpoint(respawnPoint != null ? respawnPoint : transform);
        hasActivated = true;
    }
}
