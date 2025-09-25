using UnityEngine;

public class TowerUpgrader : MonoBehaviour
{
    [Header("Setup")]
    // This will be assigned automatically when the tower is built.
    public TowerData towerData;

    // The current level of this specific tower instance.
    public int currentUpgradeLevel { get; private set; } = 0;

    // References to the tower's components
    private Turret turret;
    private Mortar mortar;
    private Flamethrower flamethrower;
    private RangeDetector rangeDetector;

    void Awake()
    {
        // Get references to the tower's main combat scripts.
        turret = GetComponent<Turret>();
        mortar = GetComponent<Mortar>();
        flamethrower = GetComponent<Flamethrower>();
        rangeDetector = GetComponentInChildren<RangeDetector>();
    }

    void Start()
    {
        // Apply the initial stats from Level 0.
        ApplyUpgrade();
    }

    public TowerUpgradeLevel GetCurrentLevelStats()
    {
        return towerData.upgradeLevels[currentUpgradeLevel];
    }

    public TowerUpgradeLevel GetNextLevelStats()
    {
        if (currentUpgradeLevel + 1 >= towerData.upgradeLevels.Length)
        {
            return null; // This is the max level
        }
        return towerData.upgradeLevels[currentUpgradeLevel + 1];
    }

    public void AttemptUpgrade()
    {
        TowerUpgradeLevel nextLevel = GetNextLevelStats();

        if (nextLevel == null)
        {
            Debug.Log("Tower is already at max level!");
            return;
        }

        if (GameManager.instance.currency >= nextLevel.upgradeCost)
        {
            GameManager.instance.SpendCurrency(nextLevel.upgradeCost);
            currentUpgradeLevel++;
            ApplyUpgrade();
            Debug.Log(gameObject.name + " upgraded to level " + (currentUpgradeLevel + 1));
        }
        else
        {
            Debug.Log("Not enough currency to upgrade!");
        }
    }

    private void ApplyUpgrade()
    {
        TowerUpgradeLevel newStats = GetCurrentLevelStats();

        // Update the Range Detector's physical collider
        if (rangeDetector != null)
        {
            rangeDetector.GetComponent<CircleCollider2D>().radius = newStats.range;
        }

        // Pass the stats to the appropriate tower script
        if (turret != null)
        {
            turret.ApplyStats(newStats);
        }
        if (mortar != null)
        {
            mortar.ApplyStats(newStats);
        }
        if (flamethrower != null)
        {
            flamethrower.ApplyStats(newStats);
        }
    }
}
