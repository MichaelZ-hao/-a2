using UnityEngine;

/// <summary>Defines a world object that can be used by the player.</summary>
public interface IInteractable
{
    string InteractionPrompt { get; }

    bool CanInteract(GameObject interactor);

    void Interact(GameObject interactor);
}
