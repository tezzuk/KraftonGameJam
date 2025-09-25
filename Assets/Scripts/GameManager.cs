using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState { BuildPhase, WaveInProgress, GameOver }
    public GameState currentState { get; private set; }
    public Animator animator;
    public string animationName = "redEffect"; // Name of the state in Animator

    [Header("Setup")]
    public WaveSpawner waveSpawner;

    [Header("Crystal Health")]
    public int crystalHealth = 10;

    [Header("Currency Settings")]
    public int startingCurrency = 150;
    public int currency { get; private set; }

    [Header("Wave Management")]
    public float buildPhaseTime = 30f; 
    private float waveTimer;
    
    [Header("Rewind Settings")]
    public int rewindsPerWave = 1,waveIndex=1;
    public int rewindsAvailable;

    [Header("Progression Perks")]
    public bool canBuildDuringWave = false;
    public int midWaveBuildLimit = 1;
    public int midWaveBuildsUsed { get; private set; }
    public UIManager uiManager;
    public GameObject WinPanel,LosePanel;

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
        SoundManager.instance.PlayDefenseMusic();
        if (currentState == GameState.BuildPhase)
        {
            currentState = GameState.WaveInProgress;
            midWaveBuildsUsed = 0;
            waveSpawner.StartNextWave();
        }
    }

    /// <summary>
    /// Called by the WaveSpawner when all enemies in a wave are defeated.
    /// </summary>
    public void WaveCompleted()
    {
        // --- NEW DEBUG LOG ---
        Debug.Log("GameManager has been notified that the wave is complete. Starting next build phase timer.");
        if (waveIndex >= waveSpawner.waves.Count)
        {
            Debug.Log("All waves completed! YOU WIN!");
            WinPanel.SetActive(true);
            return;
        }
        waveIndex++;
        EnterBuildPhase();
    }

    public void EnterBuildPhase()
    {
        SoundManager.instance.PlayBuildMusic();

        currentState = GameState.BuildPhase;
        rewindsAvailable = rewindsPerWave;
        waveTimer = buildPhaseTime; 
        uiManager.resetTimer();
    }
    
    public void EnemyReachedCrystal()
    {
        if (currentState == GameState.GameOver) return;
        crystalHealth--;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Check if the current state is NOT already the animation
        if (!stateInfo.IsName(animationName))
        {
            animator.Play(animationName, 0, 0f); // Play from start
        }
        if (crystalHealth <= 0)
        {
            GameOver();
        }
        WaveSpawner.instance.OnEnemyDied();
    }

    void GameOver()
    {
        currentState = GameState.GameOver;
        Debug.Log("GAME OVER!");
        LosePanel.SetActive(true);
        Time.timeScale = 0;
    }

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

