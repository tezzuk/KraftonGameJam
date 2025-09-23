using UnityEngine;
using System.Collections.Generic; // <-- ADD THIS LINE

public class Flamethrower : MonoBehaviour
{
    [Header("Setup")]
    public TowerData towerData; // The single source of truth for all stats
    public Transform partToRotate;
    public ParticleSystem flameEffect; // The flame particle system visual

    [Header("Live Stats")]
    public bool isBrokenDown = false;
    private float usageTime = 0; // Tracks seconds of use for the fragile mechanic
    
    // --- Private variables ---
    private GameObject currentTarget = null;
    private List<GameObject> targetsInRange = new List<GameObject>(); // <-- FIXED TYPO (Capital 'L')
    private Color originalColor;
    private SpriteRenderer towerSpriteRenderer;

    void Start()
    {
        towerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (towerSpriteRenderer != null) originalColor = towerSpriteRenderer.color;

        if (flameEffect != null)
        {
            flameEffect.Stop();
        }

        // --- AUTO-RANGE SETUP ---
        // Find the RangeDetector child and set its collider radius from our TowerData
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

        // Find a target if we don't have one
        if (currentTarget == null)
        {
            targetsInRange.RemoveAll(item => item == null);
            if (targetsInRange.Count > 0)
            {
                currentTarget = targetsInRange[0];
            }
            else
            {
                // If no targets are in range, turn off the flame effect
                if (flameEffect != null && flameEffect.isPlaying)
                {
                    flameEffect.Stop();
                }
                return;
            }
        }

        EngageTarget();
    }

    void EngageTarget()
    {
        // 1. Aim at the primary target
        Vector2 dir = currentTarget.transform.position - partToRotate.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        partToRotate.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        // 2. Turn on the flame effect
        if (flameEffect != null && !flameEffect.isPlaying)
        {
            flameEffect.Play();
        }
        
        // 3. Damage all enemies in the cone
        List<GameObject> currentTargets = new List<GameObject>(targetsInRange);
        foreach (GameObject enemyObject in currentTargets)
        {
            if (enemyObject == null) continue;

            Vector2 directionToEnemy = enemyObject.transform.position - partToRotate.position;

            // Assuming 'partToRotate.up' is the forward direction
            if (Vector2.Angle(partToRotate.up, directionToEnemy) < towerData.flameAngle / 2)
            {
                Enemy enemy = enemyObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(towerData.damagePerSecond * Time.deltaTime);
                }
            }
        }
        
        // 4. Update usage for fragile mechanic
        usageTime += Time.deltaTime;
        if (usageTime >= towerData.shotsUntilBreakdown)
        {
            BreakDown();
        }
    }

    // --- Fragile and Rewind Methods ---

    void BreakDown()
    {
        isBrokenDown = true;
        if (flameEffect != null) flameEffect.Stop();
        if (towerSpriteRenderer != null) towerSpriteRenderer.color = Color.gray;
    }

    public void Rewind()
    {
        isBrokenDown = false;
        usageTime = 0;
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

