using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Wave Settings")]
    public int enemiesPerWave = 10;
    public float spawnInterval = 2f;

    [Header("Setup")]
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public Transform[] waypoints; // Assign the path for the enemies

    // This function is called automatically when the GameObject is enabled
    void OnEnable()
    {
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        Debug.Log("SPAWNER HAS BEEN ENABLED. Starting to spawn enemies.");
        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
            // Wait for the specified interval before spawning the next one
            yield return new WaitForSeconds(spawnInterval);
        }
        
        // After the wave is done, you could tell the GameManager to go back to the build phase
        Debug.Log("Wave complete!");
        // For now, we'll just disable the spawner again.
        gameObject.SetActive(false); 
    }

    void SpawnEnemy()
    {
        GameObject newEnemyObject = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        Enemy enemyScript = newEnemyObject.GetComponent<Enemy>();

        // This is crucial: assign the waypoints to the new enemy
        if (enemyScript != null)
        {
            enemyScript.waypoints = waypoints;
        }
    }
}
