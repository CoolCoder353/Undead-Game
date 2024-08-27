using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyData
    {
        public float pointsCost;
        public GameObject enemyPrefab;
    }

    public static EnemySpawner Instance;

    public List<EnemyData> enemies;

    public Vector2 spawnArea = new Vector2(20, 20);

    public AnimationCurve spawnCurve;

    public float chanceToSpawn = 0.5f;

    public int maxSpawnInterationsPerFrame = 4;

    private float time = 0f;
    private float currentPointsSpent = 0f;

    private float minCost = 0f;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Calculate the minimum cost of an enemy
        foreach (EnemyData enemy in enemies)
        {
            if (enemy.pointsCost < minCost) minCost = enemy.pointsCost;
        }
    }

    void Update()
    {
        //Update the time in seconds
        time += Time.deltaTime;

        // Get the current position of the curve, indicating the points to spend on enemies
        float pointsToSpend = spawnCurve.Evaluate(time);
        float pointsSpent = pointsToSpend - currentPointsSpent;

        // If there are points to spend, spawn an enemy
        for (int i = 0; i < maxSpawnInterationsPerFrame; i++)
        {
            ////Debug.Log($"Points to spend: {pointsToSpend}, Points spent: {pointsSpent}, Min cost: {minCost}, Current points spent: {currentPointsSpent}");
            if (pointsSpent < minCost) break;
            // Get a random enemy from the list
            EnemyData enemy = enemies[Random.Range(0, enemies.Count)];

            // If the enemy is too expensive, skip it
            if (enemy.pointsCost > pointsSpent) continue;

            // Check if the enemy should spawn
            if (Random.value > chanceToSpawn) continue;

            // Spawn the enemy
            SpawnEnemy(enemy.enemyPrefab);

            pointsSpent -= enemy.pointsCost;
        }

        currentPointsSpent = pointsToSpend - pointsSpent;
    }


    public void EnemyDied(Enemy enemy)
    {
        // Get the enemy's cost
        float cost = 0f;
        foreach (EnemyData enemyData in enemies)
        {
            if (enemyData.enemyPrefab.GetComponent<Enemy>().ID == enemy.ID)
            {
                cost = enemyData.pointsCost;
                break;
            }
        }
        if (cost == 0)
        {
            Debug.LogError("Enemy cost not found for enemy: " + enemy.name);
            return;
        }

        // Add the cost back to the points spent
        currentPointsSpent -= cost;

        PlayerController.Instance.AddScore(cost);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnArea.x * 2, spawnArea.y * 2, 0));
    }

    void SpawnEnemy(GameObject enemy)
    {
        Camera cam = Camera.main;

        // Calculate the camera's view bounds
        float camHeight = cam.orthographicSize * 2;
        float camWidth = camHeight * cam.aspect;
        Vector3 camPosition = cam.transform.position;

        // Define the offscreen point
        Vector2 offscreenPoint = Vector2.zero;

        // Randomly choose a side to spawn the enemy offscreen
        int side = Random.Range(0, 4);

        switch (side)
        {
            case 0: // Left
                offscreenPoint = new Vector2(camPosition.x - camWidth / 2 - 1, Random.Range(camPosition.y - camHeight / 2, camPosition.y + camHeight / 2));
                break;
            case 1: // Right
                offscreenPoint = new Vector2(camPosition.x + camWidth / 2 + 1, Random.Range(camPosition.y - camHeight / 2, camPosition.y + camHeight / 2));
                break;
            case 2: // Top
                offscreenPoint = new Vector2(Random.Range(camPosition.x - camWidth / 2, camPosition.x + camWidth / 2), camPosition.y + camHeight / 2 + 1);
                break;
            case 3: // Bottom
                offscreenPoint = new Vector2(Random.Range(camPosition.x - camWidth / 2, camPosition.x + camWidth / 2), camPosition.y - camHeight / 2 - 1);
                break;
        }

        // Adjust the point relative to the spawner's position
        offscreenPoint += (Vector2)transform.position;

        // Spawn the enemy
        GameObject newEnemy = Instantiate(enemy, offscreenPoint, Quaternion.identity);
    }

}