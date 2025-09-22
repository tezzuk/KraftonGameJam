using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Turret Attributes")]
    public float range = 15f;
    public float fireRate = 1f;
    private float fireCountdown = 0f;

    [Header("Fragile Mechanic")]
    public int shotsUntilBreakdown = 20;
    public bool isBrokenDown = false;
    private int shotsFired = 0;

    [Header("Unity Setup Fields")]
    public string enemyTag = "Enemy";
    public Transform partToRotate;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public SpriteRenderer turretSpriteRenderer;
    public Color brokenColor = Color.grey;
    private Color originalColor;

    private List<GameObject> targetsInRange = new List<GameObject>();
    private GameObject currentTarget = null;

    void Start()
    {
        // Store the turret's original color so we can restore it after a rewind.
        if (turretSpriteRenderer != null)
        {
            originalColor = turretSpriteRenderer.color;
        }
    }

    void Update()
    {
        // If the turret is broken, it does nothing.
        if (isBrokenDown) return;

        // If the current target has been destroyed or has left the range, find a new one.
        if (currentTarget == null)
        {
            targetsInRange.RemoveAll(item => item == null); // Clean up the list of any dead enemies
            if (targetsInRange.Count > 0)
            {
                currentTarget = targetsInRange[0]; // Target the first enemy in the list
            }
            else
            {
                return; // If there are no targets, do nothing.
            }
        }

        LockOnAndShoot();
    }

    // --- These new methods are called by the RangeDetector script ---

    /// <summary>
    /// Called by the child RangeDetector when an object enters its trigger.
    /// </summary>
    public void OnEnemyEnteredRange(GameObject enemy)
    {
        if (enemy.CompareTag(enemyTag) && !targetsInRange.Contains(enemy))
        {
            targetsInRange.Add(enemy);
        }
    }

    /// <summary>
    /// Called by the child RangeDetector when an object exits its trigger.
    /// </summary>
    public void OnEnemyExitedRange(GameObject enemy)
    {
        if (targetsInRange.Contains(enemy))
        {
            targetsInRange.Remove(enemy);
        }

        // If the enemy that just left was our current target, we need to find a new one.
        if (enemy == currentTarget)
        {
            currentTarget = null;
        }
    }
    
    // --- The rest of the script remains the same ---

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
        Debug.Log(gameObject.name + " has broken down!");
        if (turretSpriteRenderer != null)
        {
            turretSpriteRenderer.color = brokenColor;
        }
    }

    public void Rewind()
    {
        Debug.Log("Rewinding " + gameObject.name);
        isBrokenDown = false;
        shotsFired = 0;
        if (turretSpriteRenderer != null)
        {
            turretSpriteRenderer.color = originalColor;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}

