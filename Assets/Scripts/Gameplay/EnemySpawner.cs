using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Setup")]
    public GameObject playerCharacterPrefab;
    public int enemiesToSpawn = 4;

    [Header("Optional Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Random Spawn Settings (if no spawnPoints)")]
    public bool useRandomSpawn = true;
    public float spawnRadius = 20f;

    private void Start()
    {
        if (playerCharacterPrefab == null)
        {
            Debug.LogError("EnemySpawner: playerCharacterPrefab not assigned.");
            return;
        }

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Vector3 spawnPos;

            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                var sp = spawnPoints[i % spawnPoints.Length];
                spawnPos = sp.position;
            }
            else if (useRandomSpawn)
            {
                spawnPos = GetRandomSpawnPosition();
            }
            else
            {
                Debug.LogError("EnemySpawner: No spawn points assigned, and random spawn disabled.");
                return;
            }

            GameObject enemy = Instantiate(playerCharacterPrefab, spawnPos, Quaternion.identity);
            ConvertToEnemy(enemy);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        var terrain = Terrain.activeTerrain;

        Vector2 circle = Random.insideUnitCircle * spawnRadius;
        Vector3 center = transform.position;
        Vector3 pos = center + new Vector3(circle.x, 0f, circle.y);

        if (terrain != null)
        {
            Vector3 tPos = terrain.transform.position;
            Vector3 tSize = terrain.terrainData.size;

            pos.x = Mathf.Clamp(pos.x, tPos.x, tPos.x + tSize.x);
            pos.z = Mathf.Clamp(pos.z, tPos.z, tPos.z + tSize.z);

            float h = terrain.SampleHeight(pos) + tPos.y;
            pos.y = h;
        }
        else
        {
            // fallback: raycast from above
            Vector3 rayStart = pos + Vector3.up * 50f;
            if (Physics.Raycast(rayStart, Vector3.down, out var hit, 200f))
                pos = hit.point;
        }

        return pos;
    }


    private void ConvertToEnemy(GameObject enemy)
    {
        // Remove PlayerController
        var pc = enemy.GetComponent<PlayerController>();
        if (pc != null)
            Destroy(pc);

        // Add CharacterMotor if missing
        if (enemy.GetComponent<CharacterMotor>() == null)
            enemy.AddComponent<CharacterMotor>();

        // Add EnemyChase
        if (enemy.GetComponent<EnemyChase>() == null)
            enemy.AddComponent<EnemyChase>();

        // Disable root motion for AI
        var animator = enemy.GetComponentInChildren<Animator>();
        if (animator != null)
            animator.applyRootMotion = false;

        enemy.tag = "Enemy";
    }
}
