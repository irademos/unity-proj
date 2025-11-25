using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyChase : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float rotationSpeed = 10f;
    public float stoppingDistance = 1.5f;

    private CharacterController controller;
    private Transform target;
    private Animator animator;
    private CharacterMotor motor;
    private Vector3 lastPosition;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        motor = GetComponent<CharacterMotor>();
        lastPosition = transform.position;
    }

    private void Start()
    {
        AcquireTarget();
    }

    private void Update()
    {
        if (target == null)
        {
            AcquireTarget();
            return;
        }

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;
        float distance = toTarget.magnitude;

        // Idle if close enough
        // Idle if close enough
        if (distance <= stoppingDistance)
        {
            motor.Move(Vector3.zero);  // still apply gravity!
            UpdateAnimation(0f);
            return;
        }

        Vector3 dir = toTarget.normalized;

        // Rotate toward player
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // Move toward player
        Vector3 move = dir * moveSpeed;
        motor.Move(move);

        UpdateAnimation(move.magnitude);
    }

    private void AcquireTarget()
    {
        // Prefer GameManager if you have it
        if (GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            target = GameManager.Instance.Player.transform;
            return;
        }

        // Fallback: find by tag "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            target = playerObj.transform;
    }

    private void UpdateAnimation(float horizontalSpeed)
    {
        if (animator == null) return;

        // Use the same "Speed" parameter as your player controller
        animator.SetFloat("Speed", horizontalSpeed);
    }
}
