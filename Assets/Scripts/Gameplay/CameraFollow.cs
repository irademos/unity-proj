using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -8f);
    [SerializeField] private float smoothSpeed = 10f;

    private void LateUpdate()
    {
        // Auto-hook from GameManager if not set manually
        if (target == null && GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            target = GameManager.Instance.Player.transform;
        }

        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(target);
    }
}
