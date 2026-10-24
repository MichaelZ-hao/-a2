using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider))]
/// <summary>Respawns the player after entering an out-of-bounds trigger.</summary>
public sealed class FallRespawnZone : MonoBehaviour
{
    private void Reset()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerRespawnController playerRespawn =
            other.GetComponentInParent<PlayerRespawnController>();

        if (playerRespawn != null)
        {
            playerRespawn.Respawn();
        }
    }
}
