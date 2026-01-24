using System.Collections;
using UnityEngine;

/// <summary>
/// Enemy spawner component.
/// Spawns enemies randomly from the edges of the screen.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float enemySpeed = 1f; // Reduced from 2f to 1f

    [Header("Spawn Range")]
    [SerializeField] private float spawnMin = -5f;
    [SerializeField] private float spawnMax = 5f;

    private bool isSpawning = true;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPosition;

        // Randomly spawn from one of the four edges
        int edge = Random.Range(0, 4);
        switch (edge)
        {
            case 0: // Right edge
                spawnPosition = new Vector3(spawnMax, Random.Range(spawnMin, spawnMax), 0);
                break;
            case 1: // Top edge
                spawnPosition = new Vector3(Random.Range(spawnMin, spawnMax), spawnMax, 0);
                break;
            case 2: // Left edge
                spawnPosition = new Vector3(spawnMin, Random.Range(spawnMin, spawnMax), 0);
                break;
            default: // Bottom edge
                spawnPosition = new Vector3(Random.Range(spawnMin, spawnMax), spawnMin, 0);
                break;
        }

        // Create enemy object
        GameObject enemyObj = new GameObject("Enemy");
        enemyObj.tag = "Enemy";

        Enemy enemy = enemyObj.AddComponent<Enemy>();
        enemy.Initialize(spawnPosition, enemySpeed);

        Debug.Log($"Enemy spawned at: {spawnPosition}");
    }

    public void SetSpawning(bool value)
    {
        isSpawning = value;
    }
}
