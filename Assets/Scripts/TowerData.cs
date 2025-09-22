using UnityEngine;

// This attribute allows us to create instances of this object from the Unity menu.
[CreateAssetMenu(fileName = "New Tower", menuName = "Towers/Tower Data")]
public class TowerData : ScriptableObject
{
    [Header("Tower Info")]
    public string towerName;
    public GameObject towerPrefab; // The actual tower prefab (with the Turret.cs script)

    [Header("Building Stats")]
    public int buildCost;
    // You can add more stats here later, like upgrade costs, etc.
}
