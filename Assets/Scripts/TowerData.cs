using UnityEngine;

// This new class will define the stats for a single upgrade level.
[System.Serializable]
public class TowerUpgradeLevel
{
    public int upgradeCost;
    public float range;
    public float fireRate; // For Turret/Mortar
    public int shotsUntilBreakdown;

    [Header("Projectile Damage (if applicable)")]
    public int projectileDamage; // For Turret's Projectile or Mortar's Shell

    [Header("Flamethrower Damage (if applicable)")]
    public float damagePerSecond;
    public float flameAngle;
}


[CreateAssetMenu(fileName = "New Tower", menuName = "Towers/Tower Data")]
public class TowerData : ScriptableObject
{
    [Header("Tower Info")]
    public string towerName;
    public GameObject towerPrefab;
    public int buildCost;

    [Header("Upgrade Path")]
    // This array will hold all the upgrade levels. Level 0 is the base tower.
    public TowerUpgradeLevel[] upgradeLevels;
}
