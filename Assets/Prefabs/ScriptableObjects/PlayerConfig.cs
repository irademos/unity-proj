using UnityEngine;

[CreateAssetMenu(menuName = "Config/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    public float moveSpeed = 6f;
    public float jumpForce = 3f;
    public float gravity = -9.81f;
}
