using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
/// <summary>Moves, rotates, jumps, and applies gravity to the player.</summary>
public sealed class PlayerMovementController : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private PlayerInputReader inputReader;
    [Tooltip("Assign the Main Camera. Movement is relative to its facing direction.")]
    [SerializeField] private Transform cameraTransform;

    [Header("Horizontal Movement")]
    [Min(0f)] [SerializeField] private float walkSpeed = 4f;
    [Min(0f)] [SerializeField] private float sprintSpeed = 6.5f;
    [Min(0f)] [SerializeField] private float acceleration = 20f;
    [Min(0f)] [SerializeField] private float rotationSpeed = 12f;

    [Header("Jump and Gravity")]
    [Min(0f)] [SerializeField] private float jumpHeight = 1.4f;
    [Tooltip("Keep this value negative.")]
    [SerializeField] private float gravity = -25f;
    [Tooltip("A small downward force that keeps the controller grounded on slopes.")]
    [SerializeField] private float groundedForce = -2f;

    private CharacterController characterController;
    private float currentHorizontalSpeed;
    private float verticalSpeed;
    private bool wasGrounded;

    public float MoveAmount { get; private set; }
    public float CurrentSpeed => currentHorizontalSpeed;
    public float NormalizedSpeed => sprintSpeed > 0f ? currentHorizontalSpeed / sprintSpeed : 0f;
    public float VerticalSpeed => verticalSpeed;
    public bool IsGrounded { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool LandedThisFrame { get; private set; }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (inputReader == null)
        {
            inputReader = GetComponent<PlayerInputReader>();
        }

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        if (inputReader == null)
        {
            Debug.LogError("PlayerMovementController could not find a PlayerInputReader.", this);
            enabled = false;
            return;
        }

        if (gravity >= 0f)
        {
            gravity = -Mathf.Abs(gravity == 0f ? 25f : gravity);
        }
    }

    private void Start()
    {
        IsGrounded = characterController.isGrounded;
        wasGrounded = IsGrounded;
    }

    private void Update()
    {
        IsGrounded = characterController.isGrounded;
        LandedThisFrame = !wasGrounded && IsGrounded;

        if (IsGrounded && verticalSpeed < 0f)
        {
            verticalSpeed = groundedForce;
        }

        if (IsGrounded && inputReader.JumpPressedThisFrame)
        {
            verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
            IsGrounded = false;
        }

        Vector2 moveInput = Vector2.ClampMagnitude(inputReader.Move, 1f);
        MoveAmount = moveInput.magnitude;
        IsSprinting = inputReader.SprintHeld && MoveAmount > 0.1f;

        float targetSpeed = MoveAmount * (IsSprinting ? sprintSpeed : walkSpeed);
        currentHorizontalSpeed = Mathf.MoveTowards(
            currentHorizontalSpeed,
            targetSpeed,
            acceleration * Time.deltaTime);

        Vector3 moveDirection = GetCameraRelativeDirection(moveInput);
        RotateTowardsMovement(moveDirection);

        verticalSpeed += gravity * Time.deltaTime;
        Vector3 velocity = moveDirection * currentHorizontalSpeed + Vector3.up * verticalSpeed;
        CollisionFlags collisionFlags = characterController.Move(velocity * Time.deltaTime);

        IsGrounded = characterController.isGrounded
            || (collisionFlags & CollisionFlags.Below) != 0;
        LandedThisFrame = !wasGrounded && IsGrounded;
        wasGrounded = IsGrounded;
    }

    private Vector3 GetCameraRelativeDirection(Vector2 input)
    {
        Vector3 forward = cameraTransform != null ? cameraTransform.forward : Vector3.forward;
        Vector3 right = cameraTransform != null ? cameraTransform.right : Vector3.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 direction = forward * input.y + right * input.x;
        return direction.sqrMagnitude > 1f ? direction.normalized : direction;
    }

    private void RotateTowardsMovement(Vector3 moveDirection)
    {
        if (moveDirection.sqrMagnitude < 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime);
    }

    public void ResetMotion()
    {
        currentHorizontalSpeed = 0f;
        verticalSpeed = groundedForce;
        MoveAmount = 0f;
        IsSprinting = false;
        LandedThisFrame = false;
        IsGrounded = characterController.isGrounded;
        wasGrounded = IsGrounded;
    }
}
