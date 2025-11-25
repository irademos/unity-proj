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

        // If we have a Terrain, use its true world bounds
        if (terrain != null)
        {
            Vector3 tPos = terrain.transform.position;
            Vector3 tSize = terrain.terrainData.size;

            // Random X/Z inside terrain rectangle
            float x = Random.Range(tPos.x, tPos.x + tSize.x);
            float z = Random.Range(tPos.z, tPos.z + tSize.z);

            // Start ray above max height
            Vector3 rayStart = new Vector3(x, tPos.y + tSize.y + 50f, z);

            if (Physics.Raycast(rayStart, Vector3.down, out var hit, tSize.y + 100f))
            {
                return hit.point;  // exact surface (terrain collider or anything on top)
            }

            // Fallback: sample height from terrain data
            float h = terrain.SampleHeight(new Vector3(x, 0f, z)) + tPos.y;
            return new Vector3(x, h, z);
        }
        else
        {
            // No Terrain in scene: random around spawner + raycast down
            Vector2 circle = Random.insideUnitCircle * spawnRadius;
            Vector3 pos = transform.position + new Vector3(circle.x, 50f, circle.y);

            if (Physics.Raycast(pos, Vector3.down, out var hit, 200f))
            {
                return hit.point;
            }

            // If still nothing, just drop them at spawner height
            pos.y = transform.position.y;
            return pos;
        }
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
