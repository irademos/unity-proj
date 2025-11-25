using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerConfig config;
    [SerializeField] private float rotationSpeed = 10f; 

    private CharacterController controller;
    private Vector3 velocity;
    private Animator animator;
    private CharacterMotor motor;
    private Vector3 _lastPosition;

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
        _lastPosition = transform.position;
    }

    private void Update()
    {
        HandleMovement();
        HandleJumpInput();
        UpdateAnimation();
    }


    private Vector3 lastMove; // store move vector for anim

    private void HandleMovement()
    {
        // Old or new input, keep whatever you're using:
        float h = Input.GetAxisRaw("Horizontal"); // or from new input system
        float v = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(h, v);
        if (input.sqrMagnitude > 1f)
            input.Normalize();

        // If no input, no movement or rotation
        if (input.sqrMagnitude < 0.0001f)
        {
            return;
        }

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

        // Move
        Vector3 move = moveDir * config.moveSpeed;
        controller.Move(move * Time.deltaTime);

        // Rotate character toward movement direction
        Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
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

    private void UpdateAnimation()
    {
        if (animator == null) return;

        // Position delta per frame → speed (units/sec)
        Vector3 delta = transform.position - _lastPosition;
        // Ignore vertical for walking speed
        delta.y = 0f;
        float speed = delta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);

        animator.SetFloat("Speed", speed);

        _lastPosition = transform.position;
    }
}
