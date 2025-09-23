using UnityEngine;
using System.Collections.Generic;
public class Turret : MonoBehaviour
{
    [Header("Setup")]
    public TowerData towerData; // The single source of truth for stats
    public Transform partToRotate;
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Live Stats")]
    public bool isBrokenDown = false;
    private float fireCountdown = 0f;
    private int shotsFired = 0;
    
    // --- The rest of the script is largely the same ---
    private GameObject currentTarget = null;
    private List<GameObject> targetsInRange = new List<GameObject>();
    private Color originalColor;
    private SpriteRenderer towerSpriteRenderer;
    
    void Start()
    {
        towerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (towerSpriteRenderer != null) originalColor = towerSpriteRenderer.color;

        // --- NEW AUTO-RANGE SETUP ---
        // Find the RangeDetector child and set its collider radius
        RangeDetector detector = GetComponentInChildren<RangeDetector>();
        if (detector != null)
        {
            CircleCollider2D rangeCollider = detector.GetComponent<CircleCollider2D>();
            if (rangeCollider != null)
            {
                rangeCollider.radius = towerData.range;
            }
        }
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
            fireCountdown = 1f / towerData.fireRate; // Use fireRate from TowerData
        }
        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectile = projectileGO.GetComponent<Projectile>();
        if (projectile != null) projectile.Seek(currentTarget.transform);
        
        shotsFired++;
        if (shotsFired >= towerData.shotsUntilBreakdown)
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

