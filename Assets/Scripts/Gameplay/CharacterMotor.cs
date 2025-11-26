using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMotor : MonoBehaviour
{
    [Header("Gravity")]
    public float gravity = -20f;
    public float groundedVertical = -2f;

    [Header("Actions")]
    [SerializeField] private float rollSpeed = 4.5f;
    [SerializeField] private float rollDuration = 0.6f;
    [SerializeField] private float punchDuration = 0.45f;
    [SerializeField] private Animator animator;

    private CharacterController controller;
    private Vector3 verticalVelocity;
    private Vector3 rollVelocity;
    private float rollTimer;
    private float punchTimer;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    public bool IsRolling => rollTimer > 0f;
    public bool IsPunching => punchTimer > 0f;
    public bool CanMove => !IsRolling && !IsPunching;

    /// <summary>
    /// Moves the character using a horizontal velocity vector and shared gravity.
    /// </summary>
    public void Move(Vector3 horizontalVelocity)
    {
        if (controller == null) return;

        float dt = Time.deltaTime;

        TickActionTimers(dt);

        if (IsRolling)
        {
            horizontalVelocity = rollVelocity;
        }
        else if (IsPunching)
        {
            horizontalVelocity = Vector3.zero;
        }

        // Ground snap
        if (controller.isGrounded && verticalVelocity.y < 0f)
            verticalVelocity.y = groundedVertical;

        // Gravity
        verticalVelocity.y += gravity * dt;

        // Combine
        Vector3 motion = horizontalVelocity;
        motion.y = verticalVelocity.y;

        controller.Move(motion * dt);
    }

    /// <summary>
    /// Call this to apply a jump impulse.
    /// </summary>
    public void Jump(float jumpForce)
    {
        if (IsRolling || IsPunching) return;

        // v = sqrt(2 * jumpForce * -gravityEquivalent)
        verticalVelocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
    }

    /// <summary>
    /// Starts a roll in the supplied direction. Returns false if already rolling.
    /// </summary>
    public bool TryRoll(Vector3 direction)
    {
        if (IsRolling || IsPunching) return false;

        // If direction is nearly zero, roll forward
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f)
            direction = transform.forward;

        rollVelocity = direction.normalized * rollSpeed;
        rollTimer = rollDuration;

        if (animator != null)
            animator.SetTrigger("Roll");

        return true;
    }

    /// <summary>
    /// Attempts to trigger a punch action. Returns false if currently rolling or punching.
    /// </summary>
    public bool TryPunch()
    {
        if (IsRolling || IsPunching) return false;

        punchTimer = punchDuration;

        if (animator != null)
            animator.SetTrigger("Punch");

        return true;
    }

    private void TickActionTimers(float dt)
    {
        if (rollTimer > 0f)
        {
            rollTimer -= dt;
            if (rollTimer <= 0f)
            {
                rollVelocity = Vector3.zero;
            }
        }

        if (punchTimer > 0f)
        {
            punchTimer -= dt;
        }
    }
}
