using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    // Singleton pattern for easy access from other scripts
    public static GameManager instance;

    public enum GameState { BuildPhase, WaveInProgress }
    public GameState currentState { get; private set; }

    [Header("Currency Settings")]
    public int startingCurrency = 150;
    public int currency { get; private set; }

    [Header("Rewind Settings")]
    public int rewindsPerWave = 1;
    private int rewindsAvailable;
    
    [Header("Progression Perks")]
    public bool canBuildDuringWave = false;
    public int midWaveBuildLimit = 1;
    public int midWaveBuildsUsed { get; private set; }

    [Header("Wave Management")]
    public float buildPhaseTime = 30f; 
    private float waveTimer;
    public UnityEvent OnWaveStart;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currency = startingCurrency;
        EnterBuildPhase();
    }

    void Update()
    {
        if (currentState == GameState.BuildPhase)
        {
            waveTimer -= Time.deltaTime;
            if (waveTimer <= 0f)
            {
                StartWave();
            }
        }
    }

    public void EnterBuildPhase()
    {
        currentState = GameState.BuildPhase;
        rewindsAvailable = rewindsPerWave;
        waveTimer = buildPhaseTime; 
        Debug.Log("Entering Build Phase. You have " + buildPhaseTime + " seconds. Currency: " + currency);
    }

    public void StartWave()
    {
        if (currentState == GameState.BuildPhase)
        {
            currentState = GameState.WaveInProgress;
            midWaveBuildsUsed = 0; // Reset the build counter at the start of each wave
            Debug.Log("Wave Started!");
            OnWaveStart.Invoke();
        }
    }

    // --- Currency Methods ---
    public void AddCurrency(int amount)
    {
        currency += amount;
        Debug.Log("Gained " + amount + " currency. Total: " + currency);
    }

    public void SpendCurrency(int amount)
    {
        currency -= amount;
        Debug.Log("Spent " + amount + " currency. Remaining: " + currency);
    }

    // --- Build Perk Methods ---
    public void NotifyTowerPlacedMidWave()
    {
        if (currentState == GameState.WaveInProgress)
        {
            midWaveBuildsUsed++;
        }
    }
    
    // --- Rewind Methods ---
    public bool UseRewindCharge()
    {
        if (rewindsAvailable > 0)
        {
            rewindsAvailable--;
            Debug.Log("Rewind charge used. Charges remaining: " + rewindsAvailable);
            return true;
        }
        
        Debug.Log("No rewind charges left!");
        return false;
    }
}

