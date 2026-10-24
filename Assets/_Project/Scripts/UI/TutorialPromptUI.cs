using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(TMP_Text))]
/// <summary>Displays the basic controls briefly, then fades the text out.</summary>
public sealed class TutorialPromptUI : MonoBehaviour
{
    [Header("Prompt Content")]
    [TextArea]
    [SerializeField] private string instructions =
        "WASD  Move    Mouse  Look    Shift  Sprint    Space  Jump    E  Interact";

    [Header("Display Time")]
    [Min(0f)] [SerializeField] private float displayDuration = 8f;
    [Min(0.01f)] [SerializeField] private float fadeDuration = 1f;

    private TMP_Text tutorialText;
    private Color visibleColor;
    private float elapsedTime;

    private void Awake()
    {
        tutorialText = GetComponent<TMP_Text>();
        visibleColor = tutorialText.color;
        tutorialText.text = instructions;
    }

    private void OnEnable()
    {
        elapsedTime = 0f;

        if (tutorialText != null)
        {
            tutorialText.color = visibleColor;
        }
    }

    private void Update()
    {
        elapsedTime += Time.unscaledDeltaTime;

        if (elapsedTime <= displayDuration)
        {
            return;
        }

        float fadeProgress = (elapsedTime - displayDuration) / fadeDuration;
        Color fadedColor = visibleColor;
        fadedColor.a = Mathf.Lerp(visibleColor.a, 0f, fadeProgress);
        tutorialText.color = fadedColor;

        if (fadeProgress >= 1f)
        {
            gameObject.SetActive(false);
        }
    }
}
