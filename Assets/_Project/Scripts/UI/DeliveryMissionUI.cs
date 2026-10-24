using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
/// <summary>Displays the current delivery objective, timer, and result.</summary>
public sealed class DeliveryMissionUI : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private DeliveryMissionController mission;
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text resultText;

    [Header("Timer Colours")]
    [SerializeField] private Color normalTimerColor = Color.white;
    [SerializeField] private Color warningTimerColor = new Color(1f, 0.25f, 0.2f);
    [Min(0f)] [SerializeField] private float warningTime = 10f;

    private void Awake()
    {
        if (mission == null || objectiveText == null || timerText == null || resultText == null)
        {
            Debug.LogError("DeliveryMissionUI requires the Mission and all text references.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        switch (mission.State)
        {
            case DeliveryMissionState.WaitingForPickup:
                objectiveText.text = "Objective: Collect the order from the YELLOW restaurant";
                timerText.text = "Time: --";
                timerText.color = normalTimerColor;
                resultText.text = string.Empty;
                break;

            case DeliveryMissionState.CarryingOrder:
                objectiveText.text = "Objective: Deliver the order to the GREEN customer";
                timerText.text = $"Time: {Mathf.CeilToInt(mission.RemainingTime)}";
                timerText.color = mission.RemainingTime <= warningTime
                    ? warningTimerColor
                    : normalTimerColor;
                resultText.text = string.Empty;
                break;

            case DeliveryMissionState.Completed:
                objectiveText.text = "Objective complete";
                timerText.text = $"Time left: {mission.RemainingTime:0.0}";
                timerText.color = normalTimerColor;
                resultText.text = "DELIVERY COMPLETE!\n<size=32>Press R to restart</size>";
                break;

            case DeliveryMissionState.Failed:
                objectiveText.text = "The order was not delivered in time";
                timerText.text = "Time: 0";
                timerText.color = warningTimerColor;
                resultText.text = "ORDER FAILED\n<size=32>Press R to restart</size>";
                break;
        }
    }
}
