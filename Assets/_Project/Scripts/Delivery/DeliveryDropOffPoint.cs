using UnityEngine;

[DisallowMultipleComponent]
/// <summary>Lets the player deliver a collected order to the customer.</summary>
public sealed class DeliveryDropOffPoint : MonoBehaviour, IInteractable
{
    [Tooltip("Assign the DeliveryMission object from the scene.")]
    [SerializeField] private DeliveryMissionController mission;

    public string InteractionPrompt => "Press E to deliver order";

    public bool CanInteract(GameObject interactor)
    {
        return mission != null && mission.State == DeliveryMissionState.CarryingOrder;
    }

    public void Interact(GameObject interactor)
    {
        if (mission == null)
        {
            Debug.LogError("The drop-off point has no Delivery Mission Controller assigned.", this);
            return;
        }

        mission.TryDeliverOrder();
    }
}
