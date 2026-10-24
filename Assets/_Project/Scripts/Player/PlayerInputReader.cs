using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
/// <summary>Reads player actions from the shared Input System asset.</summary>
public sealed class PlayerInputReader : MonoBehaviour
{
    [Header("Input Settings")]
    [Tooltip("Assign the InputSystem_Actions asset from the Assets folder.")]
    [SerializeField] private InputActionAsset inputActions;

    private InputActionMap playerMap;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction interactAction;

    public Vector2 Move => moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
    public Vector2 Look => lookAction?.ReadValue<Vector2>() ?? Vector2.zero;
    public bool JumpPressedThisFrame => jumpAction != null && jumpAction.WasPressedThisFrame();
    public bool SprintHeld => sprintAction != null && sprintAction.IsPressed();
    public bool InteractPressedThisFrame => interactAction != null && interactAction.WasPressedThisFrame();
    public bool IsLookFromMouse => lookAction?.activeControl?.device is Mouse;

    private void Awake()
    {
        if (inputActions == null)
        {
            Debug.LogError("PlayerInputReader requires the InputSystem_Actions asset.", this);
            enabled = false;
            return;
        }

        playerMap = inputActions.FindActionMap("Player", true);
        moveAction = playerMap.FindAction("Move", true);
        lookAction = playerMap.FindAction("Look", true);
        jumpAction = playerMap.FindAction("Jump", true);
        sprintAction = playerMap.FindAction("Sprint", true);
        interactAction = playerMap.FindAction("Interact", true);
    }

    private void OnEnable()
    {
        playerMap?.Enable();
    }

    private void OnDisable()
    {
        playerMap?.Disable();
    }
}
