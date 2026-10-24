using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
/// <summary>Stores the latest checkpoint and returns the player to it.</summary>
public sealed class PlayerRespawnController : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private PlayerMovementController movement;

    private CharacterController characterController;
    private Vector3 respawnPosition;
    private Quaternion respawnRotation;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (movement == null)
        {
            movement = GetComponent<PlayerMovementController>();
        }

        respawnPosition = transform.position;
        respawnRotation = transform.rotation;
    }

    public void SetCheckpoint(Transform checkpoint)
    {
        if (checkpoint == null)
        {
            return;
        }

        respawnPosition = checkpoint.position;
        respawnRotation = checkpoint.rotation;
        Debug.Log($"Checkpoint activated: {checkpoint.name}", checkpoint);
    }

    public void Respawn()
    {
        characterController.enabled = false;
        transform.SetPositionAndRotation(respawnPosition, respawnRotation);
        characterController.enabled = true;
        movement?.ResetMotion();

        Debug.Log("Player returned to the latest checkpoint.", this);
    }
}
