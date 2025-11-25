using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private PlayerController playerPrefab;
    [SerializeField] private Transform playerSpawnPoint;

    private void Start()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("GameBootstrap: PlayerPrefab is not assigned.");
            return;
        }

        var spawnPos = playerSpawnPoint != null 
            ? playerSpawnPoint.position 
            : Vector3.zero;

        var spawnRot = playerSpawnPoint != null 
            ? playerSpawnPoint.rotation 
            : Quaternion.identity;

        var playerInstance = Instantiate(playerPrefab, spawnPos, spawnRot);
        GameManager.Instance.RegisterPlayer(playerInstance);
    }
}
