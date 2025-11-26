using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CharacterMotor))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerConfig config;
    [SerializeField] private float rotationSpeed = 10f;

    private CharacterController controller;
    private Animator animator;
    private CharacterMotor motor;
    private Vector3 _lastPosition;
    private Vector3 _lastMoveDirection = Vector3.forward;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>(); // or GetComponent<Animator>() if Animator is on same object
        motor = GetComponent<CharacterMotor>();

        if (config == null)
        {
            Debug.LogWarning("PlayerController: PlayerConfig not assigned. Using defaults.");
            config = ScriptableObject.CreateInstance<PlayerConfig>();
        }
        if (motor != null)
        {
            motor.gravity = config.gravity;
        }
        _lastPosition = transform.position;
    }

    private void Update()
    {
        HandleMovement();
        HandleJumpInput();
        HandleActions();
        UpdateAnimation();
    }
    private void HandleMovement()
    {
        // Old or new input, keep whatever you're using:
        float h = Input.GetAxisRaw("Horizontal"); // or from new input system
        float v = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(h, v);
        if (input.sqrMagnitude > 1f)
            input.Normalize();

        // Camera-relative movement
        Transform cam = Camera.main.transform;
        Vector3 camForward = cam.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cam.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 moveDir = camForward * input.y + camRight * input.x;
        moveDir.Normalize();
        moveDir.y = 0f;

        if (moveDir.sqrMagnitude > 0.0001f)
            _lastMoveDirection = moveDir;

        Vector3 desiredVelocity = moveDir * config.moveSpeed;
        if (motor != null)
            motor.Move(desiredVelocity);
        else
            controller.Move(desiredVelocity * Time.deltaTime);

        // Rotate character toward movement direction
        if (motor == null || motor.CanMove)
        {
            if (moveDir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            motor.Jump(config.jumpForce);
            if (animator != null)
                animator.SetTrigger("Jump");
        }
    }

    private void HandleActions()
    {
        if (motor == null) return;

        // Punch mapped to default "Fire1" (Left Mouse / Ctrl)
        if (Input.GetButtonDown("Fire1"))
        {
            motor.TryPunch();
        }

        // Roll mapped to default "Fire3" (Left Shift)
        if (Input.GetButtonDown("Fire2"))
        {
            motor.TryRoll(_lastMoveDirection);
        }
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        // Position delta per frame → speed (units/sec)
        Vector3 delta = transform.position - _lastPosition;
        // Ignore vertical for walking speed
        delta.y = 0f;
        float speed = delta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);

        // Normalize by configured move speed so animator thresholds at 0.1/0.8 work as intended
        float normalizedSpeed = speed;
        if (config != null && config.moveSpeed > 0f)
            normalizedSpeed = Mathf.Clamp01(speed / config.moveSpeed);

        animator.SetFloat("Speed", normalizedSpeed);

        _lastPosition = transform.position;
    }
}
