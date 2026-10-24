using UnityEngine;

[DisallowMultipleComponent]
/// <summary>Orbits a third-person camera around the player target.</summary>
public sealed class ThirdPersonCameraController : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private PlayerInputReader inputReader;
    [Tooltip("Assign the root Player object.")]
    [SerializeField] private Transform target;

    [Header("Camera Position")]
    [SerializeField] private Vector3 pivotOffset = new Vector3(0f, 1.6f, 0f);
    [Min(0.1f)] [SerializeField] private float distance = 4.5f;

    [Header("View Controls")]
    [Min(0f)] [SerializeField] private float mouseSensitivity = 0.12f;
    [Min(0f)] [SerializeField] private float gamepadLookSpeed = 120f;
    [SerializeField] private float minimumPitch = -35f;
    [SerializeField] private float maximumPitch = 70f;
    [SerializeField] private bool lockCursorOnStart = true;

    private float yaw;
    private float pitch;

    private void Awake()
    {
        if (inputReader == null)
        {
            inputReader = FindFirstObjectByType<PlayerInputReader>();
        }

        if (target == null && inputReader != null)
        {
            target = inputReader.transform;
        }

        if (inputReader == null || target == null)
        {
            Debug.LogError("ThirdPersonCameraController requires a Player Input Reader and a Target.", this);
            enabled = false;
            return;
        }

        Vector3 startingAngles = transform.eulerAngles;
        yaw = startingAngles.y;
        pitch = NormalizeAngle(startingAngles.x);
    }

    private void Start()
    {
        SetCursorLocked(lockCursorOnStart);
    }

    private void LateUpdate()
    {
        Vector2 lookInput = inputReader.Look;
        float lookMultiplier = inputReader.IsLookFromMouse
            ? mouseSensitivity
            : gamepadLookSpeed * Time.deltaTime;

        yaw += lookInput.x * lookMultiplier;
        pitch -= lookInput.y * lookMultiplier;
        pitch = Mathf.Clamp(pitch, minimumPitch, maximumPitch);

        Quaternion cameraRotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 pivotPosition = target.position + pivotOffset;

        transform.SetPositionAndRotation(
            pivotPosition - cameraRotation * Vector3.forward * distance,
            cameraRotation);
    }

    public void SetCursorLocked(bool isLocked)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }

    private void OnDisable()
    {
        if (lockCursorOnStart)
        {
            SetCursorLocked(false);
        }
    }

    private static float NormalizeAngle(float angle)
    {
        return angle > 180f ? angle - 360f : angle;
    }
}
