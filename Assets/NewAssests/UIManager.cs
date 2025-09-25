using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject rewindImage1, rewindImage2;
    public static UIManager Instance;  // Singleton reference 
    public TMP_Text FuelText, CurrencyText, WaveText, turrentNo, MortorNo, FlameNO, timerText, BuildText,healthText, rewindText;
    public GameManager gameManager;
    public TimeCrystal timeCrystal;
    public WaveSpawner waveSpawner;
    public SpriteRenderer spriteS; // Assign sprite S in Inspector
    public Transform spriteD;      // Assign sprite D in Inspector
    public Animator animator;
    private bool animationPlayed = false;
    private float timer;
    
    public float fuel = 0;
    private void Awake()
    {
        // Make sure only one UIManager exists
        if (Instance == null)
        {
            Instance = this;
        }
        
    }
    void Start()
    {
        timer = gameManager.buildPhaseTime; // initialize correctly
    }

    void Update()
    {
        float t = Mathf.InverseLerp(10, 0, gameManager.crystalHealth);  
        if (spriteS != null)
        {
            spriteS.color = Color.Lerp(Color.green, Color.red, t);
        }

        // 2. Scale change for sprite D (0 -> 5.7 on X)
        if (spriteD != null)
        {
            float newScaleX = Mathf.Lerp(0f, 5.7f, t);
            Vector3 scale = spriteD.localScale;
            scale.x = newScaleX;
            spriteD.localScale = scale;
        }
        if (gameManager.rewindsAvailable > 0)
        {
            rewindImage1.SetActive(true);
            rewindImage2.SetActive(false);
        }
        else
        {
            rewindImage1.SetActive(false);
            rewindImage2.SetActive(true);
        }
        rewindText.text = " x " + gameManager.rewindsAvailable.ToString();
        healthText.text = gameManager.crystalHealth.ToString() + "0%";
        FuelText.text = "Fuel: " + Mathf.FloorToInt(fuel).ToString();
        CurrencyText.text = "Money: " + gameManager.currency.ToString();
        turrentNo.text = timeCrystal.connectedTurrets.Count.ToString() + "/" + timeCrystal.maxTurretConnections.ToString();
        MortorNo.text = timeCrystal.connectedMortars.Count.ToString() + "/" + timeCrystal.maxMortarConnections.ToString();
        FlameNO.text = timeCrystal.connectedFlamethrowers.Count.ToString()+"/"+timeCrystal.maxFlamethrowerConnections.ToString();
        if (waveSpawner.currentWaveIndex < waveSpawner.waves.Count)
        {
            WaveText.text = "Wave: " + (gameManager.waveIndex).ToString() + "/" + waveSpawner.waves.Count.ToString();
            if (timer > 0.1f)
            {
                timer -= Time.deltaTime;
                // Update UI
                // Timer text
                int minutes = Mathf.CeilToInt(timer) / 60;
                int seconds = Mathf.CeilToInt(timer) % 60;
                timerText.text = minutes + ":" + seconds.ToString("00");
                BuildText.text = "Build Phase";
                BuildText.color = Color.red;
                // Play animation and turn text red at 3 seconds
                if (!animationPlayed && timer <= 4f)
                {
                    animator.SetTrigger("PlayAnimation");
                    animationPlayed = true;

                }
                if (timer <= 3f)
                {
                    timerText.color = Color.red;
                }
            }
            else
            {
                timerText.text = "";
                BuildText.text = "Defend Phase";
                BuildText.color = Color.green;
            }
        }
    }

    public void resetTimer()
    {
        timer = gameManager.buildPhaseTime;
        animationPlayed = false; // reset for next countdown
        timerText.color = Color.white; // reset color
    }
    public void AddFuel(float amount)
    {
        fuel += amount;
    }
}
