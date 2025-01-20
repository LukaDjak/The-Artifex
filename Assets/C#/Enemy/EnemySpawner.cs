using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    [SerializeField] private int totalEnemiesToSpawn;
    [SerializeField] private int phases;
    [SerializeField] private float spawnMinRadius = 10f; //minimum spawn distance from the player
    [SerializeField] private float spawnMaxRadius = 20f; //maximum spawn distance from the player
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float fightDuration; //duration of the fight phase
    [SerializeField] private float restPeriod = 5f; //duration of rest periods

    [System.Serializable]
    public class EnemyType
    {
        public GameObject enemyPrefab;
        public float spawnProbability; //probability of spawning this type
    }
    [Header("Enemy Types")]
    [SerializeField] private List<EnemyType> enemyTypes = new();

    [Header("Map Boundaries")]
    [SerializeField] private Vector2 mapMinBounds; //bottom-left corner of the map
    [SerializeField] private Vector2 mapMaxBounds; //top-right corner of the map

    private Transform player;
    private Camera mainCamera;
    private int enemiesPerPhase; // = totalEnemies / phases 
    private int enemiesSpawnedInPhase = 0; //count of spawned enemies in the current phase
    private int currentPhase = 0; //current phase index
    //private bool isResting = false;
    private bool stopSpawning = false; //stops spawning when needed
    private float fightTimer = 0f;
    private readonly List<GameObject> activeEnemies = new(); //tracks active enemies

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        mainCamera = Camera.main;
        enemiesPerPhase = totalEnemiesToSpawn / phases;
        StartCoroutine(HandlePhases());
    }

    private IEnumerator HandlePhases()
    {
        while (currentPhase < phases && !stopSpawning)
        {
            //start fight phase
            fightTimer = fightDuration;
            enemiesSpawnedInPhase = 0;
            StartCoroutine(SpawnEnemiesDuringFight());

            //fight phase
            yield return new WaitForSeconds(fightDuration);

            //stop spawning and wait until all enemies are defeated
            stopSpawning = true;
            yield return new WaitUntil(() => activeEnemies.Count == 0);

            //code for rest phase - spawn the chest, ...
            Debug.Log("Rest phase started! Chest will spawn here.");

            //rest period
            yield return new WaitForSeconds(restPeriod);

            //prepare for the next phase
            stopSpawning = false;
            currentPhase++;
        }
    }

    private IEnumerator SpawnEnemiesDuringFight()
    {
        while (fightTimer > 0 && enemiesSpawnedInPhase < enemiesPerPhase && !stopSpawning)
        {
            SpawnEnemy();
            enemiesSpawnedInPhase++;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (enemyTypes.Count == 0 || player == null) return;

        //choose enemy based on probabilities
        GameObject enemyToSpawn = ChooseEnemy();
        if (enemyToSpawn == null) return;

        //calculate a valid spawn position
        Vector2 spawnPosition = GetValidSpawnPosition();
        if (spawnPosition == Vector2.zero) return;

        //spawn enemy
        GameObject newEnemy = Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
        activeEnemies.Add(newEnemy);

        //track enemy death
        newEnemy.GetComponent<Enemy>().OnDeath += () => activeEnemies.Remove(newEnemy);
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
        const int maxAttempts = 100;
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            //generate a random direction and distance
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float distance = Random.Range(spawnMinRadius, spawnMaxRadius);

            //calculate spawn position around the player
            Vector2 spawnPosition = (Vector2)player.position + randomDirection * distance;

            //check if the position is within the map bounds
            if (!IsWithinMapBounds(spawnPosition)) continue;

            //check if the position is outside of the camera view
            if (!IsVisibleToCamera(spawnPosition))
                return spawnPosition;
        }

        Debug.LogWarning("Failed to find a valid spawn position after maximum attempts.");
        return Vector2.zero; //fail-safe: Return an invalid position
    }

    private bool IsWithinMapBounds(Vector2 position)
    {
        return position.x >= mapMinBounds.x && position.x <= mapMaxBounds.x &&
               position.y >= mapMinBounds.y && position.y <= mapMaxBounds.y;
    }

    private bool IsVisibleToCamera(Vector2 position)
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(position);

        //check if the position is within the camera's visible area (0 to 1 in viewport space)
        return viewportPosition.x >= 0 && viewportPosition.x <= 1 &&
               viewportPosition.y >= 0 && viewportPosition.y <= 1 &&
               viewportPosition.z > 0; // Ensure the position is in front of the camera
    }
}