using UnityEngine;
using System.Collections.Generic;

public class Turret : MonoBehaviour
{
    [Header("Setup")]
    public Transform partToRotate;
    public GameObject projectilePrefab;
    public Transform firePoint;

    // These stats are now set by the TowerUpgrader
    [Header("Live Stats (Set by Upgrader)")]
    private float range;
    private float fireRate;
    private int shotsUntilBreakdown;
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

    // The TowerUpgrader will call this method to update the tower's stats.
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
        LockOnAndShoot();
    }

    void LockOnAndShoot()
    {
        Vector2 dir = currentTarget.transform.position - partToRotate.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        partToRotate.rotation = Quaternion.Lerp(partToRotate.rotation, rotation, Time.deltaTime * 10f);

        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }
        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectile = projectileGO.GetComponent<Projectile>();
        if (projectile != null)
        {
            // IMPORTANT: Set the damage on the projectile
            projectile.damage = this.projectileDamage;
            projectile.Seek(currentTarget.transform);
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

