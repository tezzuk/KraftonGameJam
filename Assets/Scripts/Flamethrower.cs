using UnityEngine;
using System.Collections.Generic;

public class Flamethrower : MonoBehaviour
{
    [Header("Setup")]
    public Transform partToRotate;
    public ParticleSystem flameEffect;

    [Header("Live Stats (Set by Upgrader)")]
    private float damagePerSecond;
    private float flameAngle;
    private float secondsUntilBreakdown; // Note: 'shotsUntilBreakdown' from data is used for this

    public bool isBrokenDown = false;
    private float usageTime = 0;

    private GameObject currentTarget = null;
    private List<GameObject> targetsInRange = new List<GameObject>();
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
    }

    public void ApplyStats(TowerUpgradeLevel stats)
    {
        this.damagePerSecond = stats.damagePerSecond;
        this.flameAngle = stats.flameAngle;
        this.secondsUntilBreakdown = stats.shotsUntilBreakdown;
    }

    void Update()
    {
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
        Vector2 dir = currentTarget.transform.position - partToRotate.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        partToRotate.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        if (flameEffect != null && !flameEffect.isPlaying)
        {
            flameEffect.Play();
        }

        List<GameObject> currentTargets = new List<GameObject>(targetsInRange);
        foreach (GameObject enemyObject in currentTargets)
        {
            if (enemyObject == null) continue;

            Vector2 directionToEnemy = enemyObject.transform.position - partToRotate.position;

            if (Vector2.Angle(partToRotate.up, directionToEnemy) < flameAngle / 2)
            {
                Enemy enemy = enemyObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damagePerSecond * Time.deltaTime);
                }
            }
        }

        usageTime += Time.deltaTime;
        if (usageTime >= secondsUntilBreakdown)
        {
            BreakDown();
        }
    }

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
