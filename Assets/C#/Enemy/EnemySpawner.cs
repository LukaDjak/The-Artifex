using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    [SerializeField] private int totalEnemiesToSpawn = 20; // Total enemies to spawn
    [SerializeField] private int phases = 3; // Number of phases
    [SerializeField] private float spawnMinRadius = 10f; // Minimum spawn distance from the player
    [SerializeField] private float spawnMaxRadius = 20f; // Maximum spawn distance from the player
    [SerializeField] private float restPeriod = 5f; // Duration of rest periods
    [SerializeField] private float spawnInterval = 1f; // Interval between spawns

    [System.Serializable]
    public class EnemyType
    {
        public GameObject enemyPrefab; // Enemy prefab
        public float spawnProbability; // Probability of spawning this type
    }
    [Header("Enemy Types")]
    [SerializeField] private List<EnemyType> enemyTypes = new List<EnemyType>();

    [Header("Map Boundaries")]
    [SerializeField] private Vector2 mapMinBounds; // Bottom-left corner of the map
    [SerializeField] private Vector2 mapMaxBounds; // Top-right corner of the map

    private Transform player; // Reference to the player
    private Camera mainCamera; // Reference to the main camera
    private int enemiesPerPhase; // Enemies to spawn per phase
    private int enemiesSpawnedInPhase = 0; // Count of spawned enemies in the current phase
    private int currentPhase = 0; // Current phase index
    private bool isResting = false; // Rest phase flag
    private bool stopSpawning = false; // Stops spawning when player dies

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        mainCamera = Camera.main; // Get the main camera
        enemiesPerPhase = totalEnemiesToSpawn / phases;
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (currentPhase < phases && !stopSpawning)
        {
            if (!isResting)
            {
                if (enemiesSpawnedInPhase < enemiesPerPhase)
                {
                    SpawnEnemy();
                    enemiesSpawnedInPhase++;
                    yield return new WaitForSeconds(spawnInterval);
                }
                else
                {
                    // Start rest period
                    isResting = true;
                    enemiesSpawnedInPhase = 0;
                    currentPhase++;
                    yield return new WaitForSeconds(restPeriod);
                    isResting = false;
                }
            }
            yield return null;
        }
    }

    private void SpawnEnemy()
    {
        if (enemyTypes.Count == 0 || player == null) return;

        // Choose enemy based on probabilities
        GameObject enemyToSpawn = ChooseEnemy();
        if (enemyToSpawn == null) return;

        // Calculate a valid spawn position
        Vector2 spawnPosition = GetValidSpawnPosition();
        if (spawnPosition == Vector2.zero) return; // Fail-safe: Avoid spawning if no valid position found

        // Instantiate enemy
        Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
    }

    private GameObject ChooseEnemy()
    {
        float totalProbability = 0f;
        foreach (var enemy in enemyTypes) totalProbability += enemy.spawnProbability;

        float randomValue = Random.Range(0, totalProbability);
        float cumulativeProbability = 0f;

        foreach (var enemy in enemyTypes)
        {
            cumulativeProbability += enemy.spawnProbability;
            if (randomValue <= cumulativeProbability)
                return enemy.enemyPrefab;
        }

        return null;
    }

    private Vector2 GetValidSpawnPosition()
    {
        const int maxAttempts = 100; // Prevent infinite loops
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // Generate a random direction and distance
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float distance = Random.Range(spawnMinRadius, spawnMaxRadius);

            // Calculate spawn position around the player
            Vector2 spawnPosition = (Vector2)player.position + randomDirection * distance;

            // Check if the position is within the map bounds
            if (!IsWithinMapBounds(spawnPosition)) continue;

            // Check if the position is outside of the camera view
            if (!IsVisibleToCamera(spawnPosition))
                return spawnPosition;
        }

        Debug.LogWarning("Failed to find a valid spawn position after maximum attempts.");
        return Vector2.zero; // Fail-safe: Return an invalid position
    }

    private bool IsWithinMapBounds(Vector2 position)
    {
        return position.x >= mapMinBounds.x && position.x <= mapMaxBounds.x &&
               position.y >= mapMinBounds.y && position.y <= mapMaxBounds.y;
    }

    private bool IsVisibleToCamera(Vector2 position)
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(position);

        // Check if the position is within the camera's visible area (0 to 1 in viewport space)
        return viewportPosition.x >= 0 && viewportPosition.x <= 1 &&
               viewportPosition.y >= 0 && viewportPosition.y <= 1 &&
               viewportPosition.z > 0; // Ensure the position is in front of the camera
    }

    public void StopSpawning()
    {
        stopSpawning = true;
    }
}
