using UnityEngine;
using System.Collections.Generic;

public class Mortar : MonoBehaviour
{
    [Header("Setup")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Live Stats (Set by Upgrader)")]
    private float range;
    private float fireRate;
    public int shotsUntilBreakdown;
    private int projectileDamage;

    public bool isBrokenDown = false;
    private float fireCountdown = 0f;
    private int shotsFired = 0;

    private GameObject currentTarget = null;
    private List<GameObject> targetsInRange = new List<GameObject>();
    private Color originalColor;
    private SpriteRenderer towerSpriteRenderer;

    void Start()
    {
        towerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (towerSpriteRenderer != null) originalColor = towerSpriteRenderer.color;
    }

    public void ApplyStats(TowerUpgradeLevel stats)
    {
        this.range = stats.range;
        this.fireRate = stats.fireRate;
        this.shotsUntilBreakdown = stats.shotsUntilBreakdown;
        this.projectileDamage = stats.projectileDamage;
    }

    void Update()
    {
        if (isBrokenDown) return;

        if (currentTarget == null)
        {
            targetsInRange.RemoveAll(item => item == null);
            if (targetsInRange.Count > 0) currentTarget = targetsInRange[0];
            else return;
        }

        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }
        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        GameObject shellGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        MortarShell shell = shellGO.GetComponent<MortarShell>();
        if (shell != null)
        {
            shell.damage = this.projectileDamage;
            shell.SetTarget(currentTarget.transform);
        }

        shotsFired++;
        if (shotsFired >= shotsUntilBreakdown)
        {
            BreakDown();
        }
    }

    void BreakDown()
    {
        isBrokenDown = true;
        if (towerSpriteRenderer != null) towerSpriteRenderer.color = Color.gray;
    }

    public void Rewind()
    {
        isBrokenDown = false;
        shotsFired = 0;
        if (towerSpriteRenderer != null) towerSpriteRenderer.color = originalColor;
    }

    public void OnEnemyEnteredRange(GameObject enemy)
    {
        if (enemy.CompareTag("Enemy") && !targetsInRange.Contains(enemy))
        {
            targetsInRange.Add(enemy);
        }
    }

    public void OnEnemyExitedRange(GameObject enemy)
    {
        if (targetsInRange.Contains(enemy)) targetsInRange.Remove(enemy);
        if (enemy == currentTarget) currentTarget = null;
    }
}
