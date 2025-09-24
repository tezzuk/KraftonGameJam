using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner instance;

    [Header("Setup")]
    public Transform spawnPoint;
    public Transform[] waypoints;
    public List<WaveData> waves = new List<WaveData>();

    public int currentWaveIndex = 0;
    private int enemiesRemainingInWave;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Called by the GameManager to start the next wave in the sequence.
    /// </summary>
    public void StartNextWave()
    {
        if (currentWaveIndex < waves.Count)
        {
            StartCoroutine(SpawnWave(waves[currentWaveIndex]));
        }
        else
        {
            Debug.Log("All waves completed! YOU WIN!");
            // You can add game win logic here
        }
    }

    IEnumerator SpawnWave(WaveData wave)
    {
        Debug.Log("Spawning Wave " + (currentWaveIndex + 1));
        enemiesRemainingInWave = 0;

        // Calculate the total number of enemies in this wave
        foreach (SubWave subWave in wave.subWaves)
        {
            enemiesRemainingInWave += subWave.count;
        }

        // Loop through each sub-wave in the main wave
        foreach (SubWave subWave in wave.subWaves)
        {
            // Spawn the enemies for this sub-wave
            for (int i = 0; i < subWave.count; i++)
            {
                SpawnEnemy(subWave.enemyType);
                yield return new WaitForSeconds(subWave.spawnInterval);
            }
            // Wait before starting the next sub-wave
            yield return new WaitForSeconds(wave.timeBetweenSubWaves);
        }

        currentWaveIndex++;
    }

    void SpawnEnemy(EnemyData enemyData)
    {
        GameObject enemyInstance = Instantiate(enemyData.enemyPrefab, spawnPoint.position, Quaternion.identity);
        Enemy enemyScript = enemyInstance.GetComponent<Enemy>();

        if (enemyScript != null)
        {
            enemyScript.waypoints = waypoints;
            enemyScript.Setup(enemyData);
        }
    }

    /// <summary>
    /// Called by an enemy when it dies.
    /// </summary>
    public void OnEnemyDied()
    {
        enemiesRemainingInWave--;

        if (enemiesRemainingInWave <= 0)
        {
            // All enemies in the wave are defeated
            GameManager.instance.WaveCompleted();
        }
    }
}
