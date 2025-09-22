using UnityEngine;
using System.Collections.Generic;

// This is an 'abstract' class, meaning it's a template that other scripts must inherit from.
public abstract class Tower : MonoBehaviour
{
    [Header("Fragile Mechanic")]
    public int shotsUntilBreakdown = 20;
    public bool isBrokenDown = false;
    protected int shotsFired = 0;

    [Header("Unity Setup Fields")]
    public string enemyTag = "Enemy";
    public SpriteRenderer towerSpriteRenderer;
    public Color brokenColor = Color.grey;
    protected Color originalColor;
    
    protected GameObject currentTarget = null;
    protected List<GameObject> targetsInRange = new List<GameObject>();

    protected virtual void Start()
    {
        if (towerSpriteRenderer != null)
        {
            originalColor = towerSpriteRenderer.color;
        }
    }

    protected void BreakDown()
    {
        isBrokenDown = true;
        Debug.Log(gameObject.name + " has broken down!");
        if (towerSpriteRenderer != null)
        {
            towerSpriteRenderer.color = brokenColor;
        }
    }

    // This public method can be called by the Time Crystal on any tower type
    public void Rewind()
    {
        Debug.Log("Rewinding " + gameObject.name);
        isBrokenDown = false;
        shotsFired = 0;

        // This line has been corrected from the previous version.
        if (towerSpriteRenderer != null)
        {
            towerSpriteRenderer.color = originalColor;
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
}

