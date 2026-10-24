using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
/// <summary>Shows the current interaction prompt when a valid target is available.</summary>
public sealed class InteractionPromptUI : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private PlayerInteractor playerInteractor;
    [SerializeField] private TMP_Text promptText;

    private void Awake()
    {
        if (playerInteractor == null || promptText == null)
        {
            Debug.LogError("InteractionPromptUI requires a Player Interactor and Prompt Text.", this);
            enabled = false;
            return;
        }

        promptText.enabled = false;
    }

    private void Update()
    {
        string currentPrompt = playerInteractor.GetCurrentInteractionPrompt();
        bool shouldShowPrompt = !string.IsNullOrEmpty(currentPrompt);

        promptText.enabled = shouldShowPrompt;
        if (shouldShowPrompt)
        {
            promptText.text = currentPrompt;
        }
    }
}
