using System;
using UnityEngine;

[DisallowMultipleComponent]
/// <summary>Finds interactable objects in the camera view and activates them.</summary>
public sealed class PlayerInteractor : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private PlayerInputReader inputReader;
    [Tooltip("Usually assign the Main Camera so interaction checks start from the centre of the view.")]
    [SerializeField] private Transform interactionOrigin;

    [Header("Detection Settings")]
    [Min(0.1f)] [SerializeField] private float interactionDistance = 2.5f;
    [SerializeField] private LayerMask interactionLayers = ~0;

    private void Awake()
    {
        if (inputReader == null)
        {
            inputReader = GetComponent<PlayerInputReader>();
        }

        if (interactionOrigin == null && Camera.main != null)
        {
            interactionOrigin = Camera.main.transform;
        }

        if (inputReader == null || interactionOrigin == null)
        {
            Debug.LogError("PlayerInteractor requires a Player Input Reader and an Interaction Origin.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (inputReader.InteractPressedThisFrame && !TryInteract())
        {
            Debug.Log("Interaction failed. Move closer and aim the centre of the view at an interactable object.", this);
        }
    }

    public bool TryInteract()
    {
        if (!TryFindInteractable(out IInteractable interactable))
        {
            return false;
        }

        interactable.Interact(gameObject);
        return true;
    }

    public string GetCurrentInteractionPrompt()
    {
        return TryFindInteractable(out IInteractable interactable)
            ? interactable.InteractionPrompt
            : string.Empty;
    }

    private bool TryFindInteractable(out IInteractable interactable)
    {
        interactable = null;
        Ray interactionRay = new Ray(interactionOrigin.position, interactionOrigin.forward);

        // Include the camera-to-player gap so the usable range begins at the player.
        float cameraToPlayerDistance = Vector3.Distance(interactionOrigin.position, transform.position);
        float rayDistance = cameraToPlayerDistance + interactionDistance;

        RaycastHit[] hits = Physics.RaycastAll(
            interactionRay,
            rayDistance,
            interactionLayers,
            QueryTriggerInteraction.Collide);

        Array.Sort(hits, (first, second) => first.distance.CompareTo(second.distance));

        foreach (RaycastHit hit in hits)
        {
            // Ignore the player's own colliders.
            if (hit.collider.transform.IsChildOf(transform))
            {
                continue;
            }

            MonoBehaviour[] behaviours = hit.collider.GetComponentsInParent<MonoBehaviour>(true);
            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour is IInteractable candidate)
                {
                    if (candidate.CanInteract(gameObject))
                    {
                        interactable = candidate;
                        return true;
                    }

                    return false;
                }
            }

            // A solid non-interactable object blocks anything behind it.
            return false;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (interactionOrigin == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        float cameraToPlayerDistance = Vector3.Distance(interactionOrigin.position, transform.position);
        Gizmos.DrawRay(
            interactionOrigin.position,
            interactionOrigin.forward * (cameraToPlayerDistance + interactionDistance));
    }
}
