using UnityEngine;

[DisallowMultipleComponent]
/// <summary>Lets the player collect an order at the restaurant.</summary>
public sealed class DeliveryPickupPoint : MonoBehaviour, IInteractable
{
    [Tooltip("Assign the DeliveryMission object from the scene.")]
    [SerializeField] private DeliveryMissionController mission;

    public string InteractionPrompt => "Press E to collect order";

    public bool CanInteract(GameObject interactor)
    {
        return mission != null && mission.State == DeliveryMissionState.WaitingForPickup;
    }

    public void Interact(GameObject interactor)
    {
        if (mission == null)
        {
            Debug.LogError("The pickup point has no Delivery Mission Controller assigned.", this);
            return;
        }

        mission.TryPickupOrder();
    }
}
