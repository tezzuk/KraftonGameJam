using UnityEngine;

[CreateAssetMenu(fileName = "New Tower", menuName = "Towers/Tower Data")]
public class TowerData : ScriptableObject
{
    [Header("Tower Info")]
    public string towerName;
    public GameObject towerPrefab;

    [Header("Building Stats")]
    public int buildCost;

    [Header("Combat Stats (All Towers)")]
    public float range;
    public float fireRate; // For Turret/Mortar
    public int shotsUntilBreakdown;

    [Header("Combat Stats (Flamethrower Only)")]
    public float damagePerSecond;
    public float flameAngle;
}

