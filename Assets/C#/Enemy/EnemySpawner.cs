using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    [SerializeField] private int enemiesPerFight = 10; //number of enemies to spawn during each fight phase
    [SerializeField] private float spawnMinRadius = 10f; //minimum spawn distance from the player
    [SerializeField] private float spawnMaxRadius = 20f; //maximum spawn distance from the player
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float fightDuration = 30f;
    [SerializeField] private float restPeriod = 5f;

    [System.Serializable]
    public class EnemyType
    {
        public GameObject enemyPrefab;
        public float spawnProbability;
    }

    [System.Serializable]
    public class ChestType
    {
        public GameObject chestPrefab;
        public float spawnProbability;
    }

    [Header("Enemies")]
    [SerializeField] private List<EnemyType> enemyTypes = new();

    [Header("Chests")]
    [SerializeField] private List<ChestType> chestTypes = new();

    [Header("Map Boundaries")]
    [SerializeField] private Vector2 mapMinBounds; //bottom-left corner of the map
    [SerializeField] private Vector2 mapMaxBounds; //top-right corner of the map

    private Transform player;
    private Camera mainCamera;
    private int enemiesSpawnedInFight = 0; //count of spawned enemies in the current fight
    private bool stopSpawning = false;
    private float fightTimer = 0f;
    private readonly List<GameObject> activeEnemies = new(); //tracks active enemies

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        mainCamera = Camera.main;
        StartCoroutine(HandleFightCycle());
    }

    private IEnumerator HandleFightCycle()
    {
        while (!stopSpawning)
        {
            //start fight phase
            fightTimer = fightDuration;
            enemiesSpawnedInFight = 0;
            stopSpawning = false;
            StartCoroutine(SpawnEnemiesDuringFight());

            //wait for the fight phase to end
            yield return new WaitUntil(() => enemiesSpawnedInFight >= enemiesPerFight);

            //once fight phase ends, wait for enemies to be defeated
            yield return new WaitUntil(() => activeEnemies.Count == 0);

            //spawn the chest during the rest phase
            SpawnChest();

            //give some health to player
            player.GetComponent<Player>().GiveHealth((int)(10 * player.GetComponent<Player>().chestMultipliers.maxHealthMultiplier));

            //rest period
            yield return new WaitForSeconds(restPeriod);
        }
    }

    private IEnumerator SpawnEnemiesDuringFight()
    {
        while (enemiesSpawnedInFight < enemiesPerFight && !stopSpawning)
        {
            SpawnEnemy();
            enemiesSpawnedInFight++;
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

        //instantiate enemy - use object pooling in the future
        GameObject newEnemy = Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity, transform);
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

    private void SpawnChest()
    {
        //choose chest based on probabilities
        GameObject chestToSpawn = ChooseChest();

        //calculate a valid spawn position
        Vector2 chestPosition = GetValidSpawnPosition();
        if (chestPosition == Vector2.zero)
            return;

        //spawn chest
        GameObject chest = Instantiate(chestToSpawn, chestPosition, Quaternion.identity);
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        chest.GetComponent<SpriteRenderer>().flipX = player.position.x > chest.transform.position.x;

    }

    private GameObject ChooseChest()
    {
        float totalProbability = 0f;
        foreach (var chest in chestTypes) totalProbability += chest.spawnProbability;

        float randomValue = Random.Range(0, totalProbability);
        float cumulativeProbability = 0f;

        foreach (var chest in chestTypes)
        {
            cumulativeProbability += chest.spawnProbability;
            if (randomValue <= cumulativeProbability)
                return chest.chestPrefab;
        }

        return null;
    }

    private Vector2 GetValidSpawnPosition()
    {
        while (true)
        {
            // Generate a random direction and distance
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float distance = Random.Range(spawnMinRadius, spawnMaxRadius);

            // Calculate spawn position around the player
            Vector2 spawnPosition = (Vector2)player.position + randomDirection * distance;

            // Check if the position is within the map bounds
            if (!IsWithinMapBounds(spawnPosition)) continue;

            // Check if the position is outside of the camera view
            if (!IsVisibleToCamera(spawnPosition)) continue;

            // Check for tilemap collision or overlap with obstacles
            if (IsPositionObstructed(spawnPosition)) continue;

            // Valid spawn position
            return spawnPosition;
        }
    }

    private bool IsPositionObstructed(Vector2 position)
    {
        int groundLayer = LayerMask.GetMask("Ground"); // Replace with your obstacle layer name
        Collider2D collider = Physics2D.OverlapCircle(position, 0.5f, groundLayer); // Adjust radius
        if (collider != null)
            return true;
        return false;
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
               viewportPosition.z > 0; //ensure the position is in front of the camera
    }
}