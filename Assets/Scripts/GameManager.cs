using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState { BuildPhase, WaveInProgress, GameOver }
    public GameState currentState { get; private set; }

    [Header("Setup")]
    public WaveSpawner waveSpawner; // A direct reference to the new wave spawner

    [Header("Crystal Health")]
    public int crystalHealth = 10;

    [Header("Currency Settings")]
    public int startingCurrency = 150;
    public int currency { get; private set; }

    [Header("Wave Management")]
    public float buildPhaseTime = 30f; 
    private float waveTimer;
    
    // --- Other variables for perks and rewinds ---
    [Header("Rewind Settings")]
    public int rewindsPerWave = 1;
    private int rewindsAvailable;

    [Header("Progression Perks")]
    public bool canBuildDuringWave = false;
    public int midWaveBuildLimit = 1;
    public int midWaveBuildsUsed { get; private set; }

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currency = startingCurrency;
        EnterBuildPhase();
    }

    void Update()
    {
        if (currentState == GameState.GameOver) return;

        if (currentState == GameState.BuildPhase)
        {
            waveTimer -= Time.deltaTime;
            if (waveTimer <= 0f)
            {
                StartWave();
            }
        }
    }
    
    public void StartWave()
    {
        if (currentState == GameState.BuildPhase)
        {
            currentState = GameState.WaveInProgress;
            midWaveBuildsUsed = 0;
            Debug.Log("Wave Started!");
            // Directly tell the new spawner to start the next wave
            waveSpawner.StartNextWave();
        }
    }

    /// <summary>
    /// This is called by the WaveSpawner when all enemies in a wave are defeated.
    /// </summary>
    public void WaveCompleted()
    {
        Debug.Log("Wave Completed! Returning to Build Phase.");
        EnterBuildPhase();
    }

    public void EnterBuildPhase()
    {
        currentState = GameState.BuildPhase;
        rewindsAvailable = rewindsPerWave;
        waveTimer = buildPhaseTime; 
    }
    
    public void EnemyReachedCrystal()
    {
        if (currentState == GameState.GameOver) return;
        crystalHealth--;
        if (crystalHealth <= 0) 
        {
            GameOver();
        }
        // An enemy reaching the end still counts as one less enemy for the wave.
        WaveSpawner.instance.OnEnemyDied();
    }

    void GameOver()
    {
        currentState = GameState.GameOver;
        Debug.Log("GAME OVER!");
        Time.timeScale = 0; // Pauses the game
    }

    // --- Other Helper Methods ---

    public void AddCurrency(int amount)
    {
        currency += amount;
    }

    public void SpendCurrency(int amount)
    {
        currency -= amount;
    }

    public void NotifyTowerPlacedMidWave()
    {
        if (currentState == GameState.WaveInProgress)
        {
            midWaveBuildsUsed++;
        }
    }
    
    public bool UseRewindCharge()
    {
        if (rewindsAvailable > 0)
        {
            rewindsAvailable--;
            return true;
        }
        return false;
    }
}

