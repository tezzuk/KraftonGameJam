using UnityEngine;
using System.Collections.Generic;

public class Mortar : MonoBehaviour
{
    [Header("Mortar Attributes")]
    public float range = 25f;
    public float fireRate = 0.5f;
    private float fireCountdown = 0f;

    [Header("Fragile Mechanic")]
    public int shotsUntilBreakdown = 10; // Mortars might break down faster
    public bool isBrokenDown = false;
    private int shotsFired = 0;

    [Header("Unity Setup Fields")]
    public string enemyTag = "Enemy";
    public GameObject projectilePrefab;
    public Transform firePoint;
    public SpriteRenderer mortarSpriteRenderer; // For visual feedback
    public Color brokenColor = Color.grey;
    private Color originalColor;
    
    private GameObject currentTarget = null;
    private List<GameObject> targetsInRange = new List<GameObject>();

    void Start()
    {
        // Store the original color for the rewind mechanic
        if (mortarSpriteRenderer != null)
        {
            originalColor = mortarSpriteRenderer.color;
        }
    }

    void Update()
    {
        // If the mortar is broken, it cannot do anything.
        if (isBrokenDown) return;

        if (currentTarget == null)
        {
            targetsInRange.RemoveAll(item => item == null);
            if (targetsInRange.Count > 0)
            {
                currentTarget = targetsInRange[0];
            }
            else
            {
                return;
            }
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
            shell.SetTarget(currentTarget.transform);
        }

        // Count the shot and check if it breaks down
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
        if (mortarSpriteRenderer != null)
        {
            mortarSpriteRenderer.color = brokenColor;
        }
    }

    // This public method is called by the Time Crystal to repair the mortar
    public void Rewind()
    {
        Debug.Log("Rewinding " + gameObject.name);
        isBrokenDown = false;
        shotsFired = 0;

        if (mortarSpriteRenderer != null)
        {
            mortarSpriteRenderer.color = originalColor;
        }
    }

    // These public methods are called by the child "RangeDetector" script
    public void OnEnemyEnteredRange(GameObject enemy)
    {
        if (enemy.CompareTag(enemyTag) && !targetsInRange.Contains(enemy))
        {
            targetsInRange.Add(enemy);
        }
    }

    public void OnEnemyExitedRange(GameObject enemy)
    {
        if (targetsInRange.Contains(enemy))
        {
            targetsInRange.Remove(enemy);
        }
        if (enemy == currentTarget)
        {
            currentTarget = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}

