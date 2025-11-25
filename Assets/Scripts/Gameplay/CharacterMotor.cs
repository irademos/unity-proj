using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMotor : MonoBehaviour
{
    [Header("Gravity")]
    public float gravity = -20f;
    public float groundedVertical = -2f;

    private CharacterController controller;
    private Vector3 verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    /// <summary>
    /// Moves the character using a horizontal velocity vector and shared gravity.
    /// </summary>
    public void Move(Vector3 horizontalVelocity)
    {
        if (controller == null) return;

        // Ground snap
        if (controller.isGrounded && verticalVelocity.y < 0f)
            verticalVelocity.y = groundedVertical;

        // Gravity
        verticalVelocity.y += gravity * Time.deltaTime;

        // Combine
        Vector3 motion = horizontalVelocity;
        motion.y = verticalVelocity.y;

        controller.Move(motion * Time.deltaTime);
    }

    /// <summary>
    /// Call this to apply a jump impulse.
    /// </summary>
    public void Jump(float jumpForce)
    {
        // v = sqrt(2 * jumpForce * -gravityEquivalent)
        verticalVelocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
    }
}
