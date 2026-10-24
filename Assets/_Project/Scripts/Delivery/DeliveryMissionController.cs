using UnityEngine;
using UnityEngine.InputSystem;

public enum DeliveryMissionState
{
    WaitingForPickup,
    CarryingOrder,
    Completed,
    Failed
}

[DisallowMultipleComponent]
/// <summary>Controls the pickup, timer, delivery, and mission reset flow.</summary>
public sealed class DeliveryMissionController : MonoBehaviour
{
    [Header("Mission Settings")]
    [Min(1f)] [SerializeField] private float deliveryTimeLimit = 60f;

    [Header("Restart")]
    [Tooltip("Assign the scene Player. Pressing R after the mission ends returns it to this starting position.")]
    [SerializeField] private Transform playerToReset;

    public DeliveryMissionState State { get; private set; } = DeliveryMissionState.WaitingForPickup;
    public float RemainingTime { get; private set; }
    public bool IsCarryingOrder => State == DeliveryMissionState.CarryingOrder;

    private Vector3 playerStartPosition;
    private Quaternion playerStartRotation;
    private CharacterController playerCharacterController;
    private PlayerMovementController playerMovement;
    private bool hasPlayerStartTransform;

    private void Awake()
    {
        RemainingTime = deliveryTimeLimit;
    }

    private void Start()
    {
        if (playerToReset == null)
        {
            Debug.LogWarning("DeliveryMissionController has no Player assigned. The mission can reset, but the Player will not return to the start.", this);
            return;
        }

        playerStartPosition = playerToReset.position;
        playerStartRotation = playerToReset.rotation;
        playerCharacterController = playerToReset.GetComponent<CharacterController>();
        playerMovement = playerToReset.GetComponent<PlayerMovementController>();
        hasPlayerStartTransform = true;
    }

    private void Update()
    {
        if ((State == DeliveryMissionState.Completed || State == DeliveryMissionState.Failed)
            && Keyboard.current != null
            && Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetMission();
            return;
        }

        if (State != DeliveryMissionState.CarryingOrder)
        {
            return;
        }

        RemainingTime = Mathf.Max(0f, RemainingTime - Time.deltaTime);

        if (RemainingTime <= 0f)
        {
            State = DeliveryMissionState.Failed;
            Debug.Log("Delivery failed: the order timed out.", this);
        }
    }

    public bool TryPickupOrder()
    {
        if (State != DeliveryMissionState.WaitingForPickup)
        {
            Debug.Log("The order cannot be collected now because it was already collected or the mission has ended.", this);
            return false;
        }

        State = DeliveryMissionState.CarryingOrder;
        RemainingTime = deliveryTimeLimit;
        Debug.Log($"Order collected. Deliver it within {deliveryTimeLimit:0} seconds.", this);
        return true;
    }

    public bool TryDeliverOrder()
    {
        if (State != DeliveryMissionState.CarryingOrder)
        {
            Debug.Log("Delivery failed: collect the order from the restaurant first.", this);
            return false;
        }

        State = DeliveryMissionState.Completed;
        Debug.Log($"Delivery complete with {RemainingTime:0.0} seconds remaining.", this);
        return true;
    }

    /// <summary>Removes delivery time after a route hazard hits the player.</summary>
    public void ApplyTimePenalty(float seconds)
    {
        if (State != DeliveryMissionState.CarryingOrder || seconds <= 0f)
        {
            return;
        }

        RemainingTime = Mathf.Max(0f, RemainingTime - seconds);
        Debug.Log($"Route hazard removed {seconds:0.0} seconds.", this);

        if (RemainingTime <= 0f)
        {
            State = DeliveryMissionState.Failed;
            Debug.Log("Delivery failed: a route penalty used the remaining time.", this);
        }
    }

    [ContextMenu("Reset Mission")]
    public void ResetMission()
    {
        State = DeliveryMissionState.WaitingForPickup;
        RemainingTime = deliveryTimeLimit;

        if (hasPlayerStartTransform)
        {
            if (playerCharacterController != null)
            {
                playerCharacterController.enabled = false;
            }

            playerToReset.SetPositionAndRotation(playerStartPosition, playerStartRotation);

            if (playerCharacterController != null)
            {
                playerCharacterController.enabled = true;
            }

            playerMovement?.ResetMotion();
        }

        Debug.Log("Mission reset. Go to the restaurant to collect the order.", this);
    }
}
