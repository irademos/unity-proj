using UnityEngine;
using UnityEngine.InputSystem;

public class CameraOrbitNewInput : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Settings")]
    public float distance = 6f;
    public float minDistance = 3f;
    public float maxDistance = 12f;
    public float orbitSpeed = 0.13f;
    public float zoomSpeed = 2f;

    [Header("Angles")]
    public float minVerticalAngle = -20f;
    public float maxVerticalAngle = 60f;

    private float yaw;
    private float pitch;

    // Your existing Input Actions wrapper
    private InputSystem_Actions input;

    private void OnEnable()
    {
        if (input == null)
            input = new InputSystem_Actions();

        // Enable the map you used ("Camera" or "Player")
        input.Player.Enable();
    }

    private void OnDisable()
    {
        input.Player.Disable();
    }

    private void Start()
    {
        TryAcquireTarget();

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        LockCursor();
    }

    private void LateUpdate()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            UnlockCursor();

        if (target == null)
        {
            TryAcquireTarget();
            if (target == null)
                return;
        }

        Vector2 look = input.Player.Look.ReadValue<Vector2>();
        float zoom = input.Player.Zoom.ReadValue<float>();

        // Orbit
        yaw += look.x * orbitSpeed;
        pitch = Mathf.Clamp(pitch - look.y * orbitSpeed, minVerticalAngle, maxVerticalAngle);

        // Zoom
        distance = Mathf.Clamp(distance - zoom * zoomSpeed, minDistance, maxDistance);

        // Position camera
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rot * Vector3.back * distance;

        transform.position = target.position + offset;
        transform.LookAt(target.position);
    }

    private void TryAcquireTarget()
    {
        if (GameManager.Instance?.Player != null)
        {
            target = GameManager.Instance.Player.transform;
            return;
        }

        // GameObject p = GameObject.FindWithTag("Player");
        // if (p != null)
        //     target = p.transform;
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
