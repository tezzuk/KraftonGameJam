using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemies/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Enemy Info")]
    public string enemyName;
    public GameObject enemyPrefab; // The enemy prefab with the Enemy.cs script

    [Header("Enemy Stats")]
    public float health = 10f;
    public float moveSpeed = 2f;
    public int currencyOnDeath = 10;
}
