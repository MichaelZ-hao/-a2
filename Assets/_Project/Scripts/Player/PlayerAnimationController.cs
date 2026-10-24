using UnityEngine;

[DisallowMultipleComponent]
/// <summary>Sends movement state values to the character Animator.</summary>
public sealed class PlayerAnimationController : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovementController movement;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int GroundedHash = Animator.StringToHash("Grounded");
    private static readonly int VerticalSpeedHash = Animator.StringToHash("VerticalSpeed");
    private static readonly int SprintingHash = Animator.StringToHash("Sprinting");
    private static readonly int LandHash = Animator.StringToHash("Land");
    private static readonly int DeliverHash = Animator.StringToHash("Deliver");

    private bool hasSpeed;
    private bool hasGrounded;
    private bool hasVerticalSpeed;
    private bool hasSprinting;
    private bool hasLand;
    private bool hasDeliver;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (movement == null)
        {
            movement = GetComponentInParent<PlayerMovementController>();
        }

        if (animator == null || movement == null)
        {
            Debug.LogWarning("PlayerAnimationController requires both an Animator and a Player Movement Controller.", this);
            enabled = false;
            return;
        }

        hasSpeed = HasParameter(SpeedHash, AnimatorControllerParameterType.Float);
        hasGrounded = HasParameter(GroundedHash, AnimatorControllerParameterType.Bool);
        hasVerticalSpeed = HasParameter(VerticalSpeedHash, AnimatorControllerParameterType.Float);
        hasSprinting = HasParameter(SprintingHash, AnimatorControllerParameterType.Bool);
        hasLand = HasParameter(LandHash, AnimatorControllerParameterType.Trigger);
        hasDeliver = HasParameter(DeliverHash, AnimatorControllerParameterType.Trigger);
    }

    private void Update()
    {
        if (hasSpeed)
        {
            animator.SetFloat(SpeedHash, movement.MoveAmount, 0.05f, Time.deltaTime);
        }

        if (hasGrounded)
        {
            animator.SetBool(GroundedHash, movement.IsGrounded);
        }

        if (hasVerticalSpeed)
        {
            animator.SetFloat(VerticalSpeedHash, movement.VerticalSpeed);
        }

        if (hasSprinting)
        {
            animator.SetBool(SprintingHash, movement.IsSprinting);
        }

        if (hasLand && movement.LandedThisFrame)
        {
            animator.SetTrigger(LandHash);
        }
    }

    public void TriggerDelivery()
    {
        if (hasDeliver)
        {
            animator.SetTrigger(DeliverHash);
        }
    }

    private bool HasParameter(int parameterHash, AnimatorControllerParameterType parameterType)
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.nameHash == parameterHash && parameter.type == parameterType)
            {
                return true;
            }
        }

        return false;
    }
}
