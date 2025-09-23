using UnityEngine;
using System.Collections.Generic;

public class Mortar : MonoBehaviour
{
    [Header("Setup")]
    public TowerData towerData; // The single source of truth for all stats
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Live Stats")]
    public bool isBrokenDown = false;
    private float fireCountdown = 0f;
    private int shotsFired = 0;
    
    // --- Private variables ---
    private GameObject currentTarget = null;
    private List<GameObject> targetsInRange = new List<GameObject>();
    private Color originalColor;
    private SpriteRenderer towerSpriteRenderer;
    
    void Start()
    {
        towerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (towerSpriteRenderer != null) originalColor = towerSpriteRenderer.color;

        // --- AUTO-RANGE SETUP ---
        // Find the RangeDetector child object and automatically set its
        // collider radius based on the value in our TowerData asset.
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
            else return; // No target, do nothing.
        }
        
        // Firing logic for the Mortar
        if (fireCountdown <= 0f)
        {
            Shoot();
            // Read the fireRate from the TowerData asset
            fireCountdown = 1f / towerData.fireRate;
        }
        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        GameObject shellGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        MortarShell shell = shellGO.GetComponent<MortarShell>();
        if (shell != null)
        {
            shell.SetTarget(currentTarget.transform);
        }

        shotsFired++;
        // Read the shot capacity from the TowerData asset
        if (shotsFired >= towerData.shotsUntilBreakdown)
        {
            BreakDown();
        }
    }

    // --- Fragile and Rewind Methods ---

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

    // --- Enemy Detection Methods (called by RangeDetector) ---

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

