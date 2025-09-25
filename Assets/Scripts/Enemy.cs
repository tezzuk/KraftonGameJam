using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Live Stats (Set by Spawner)")]
    public float health;
    public float moveSpeed, initialspeed;
    public int currencyOnDeath;
    public GameObject floatingTextPrefab;
    public float freezTime = 0.5f;
    private Color originalColor;

    private SpriteRenderer spriteRenderer;

    [Header("Setup")]
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private Coroutine freezeCoroutine;
    
    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Setup(EnemyData data)
    {
        health = data.health;
        moveSpeed = data.moveSpeed;
        currencyOnDeath = data.currencyOnDeath;
        initialspeed = data.moveSpeed;
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void Update()
    {
        if (waypoints != null && currentWaypointIndex < waypoints.Length)
        {
            Transform targetWaypoint = waypoints[currentWaypointIndex];
            Vector3 direction = (targetWaypoint.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            if (spriteRenderer != null)
            {
                if (Mathf.Abs(direction.x) > 0.01f)
                {
                    spriteRenderer.flipX = direction.x > 0;
                }
            }

            if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
            {
                currentWaypointIndex++;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Crystal"))
        {
            GameManager.instance.EnemyReachedCrystal();
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// For single-hit damage (Turret, Mortar). This method PLAYS A SOUND.
    /// </summary>
    public void TakeDamage(float damage)
    {
        SoundManager.instance.PlayEnemyHitSound();
        ProcessDamage(damage);
    }

    /// <summary>
    /// For continuous damage (Flamethrower). This method DOES NOT play a sound.
    /// </summary>
    public void TakeContinuousDamage(float damage)
    {
        ProcessDamage(damage);
    }

    // This is a shared private method to handle the actual health reduction and effects.
    private void ProcessDamage(float damage)
    {
        health -= damage;
        // UIManager.Instance.AddFuel(damage); // Make sure UIManager is correctly set up

        if (damage > 1.0f)
        {
            ShowFloatingText( Mathf.RoundToInt(damage).ToString());
            if (freezeCoroutine != null) StopCoroutine(freezeCoroutine);
            freezeCoroutine = StartCoroutine(FreezeCoroutine());
        }

        if (health <= 0)
        {
            Die();
        }
    }

    IEnumerator FreezeCoroutine()
    {
        if (spriteRenderer != null) spriteRenderer.color = new Color(0.8f, 0f, 0f);
        moveSpeed = 0;
        yield return new WaitForSeconds(freezTime);
        moveSpeed = initialspeed;
        if (spriteRenderer != null) spriteRenderer.color = originalColor;
        freezeCoroutine = null;
    }
    
    void Die()
    {
        SoundManager.instance.PlayEnemyDeathSound();
        GameManager.instance.AddCurrency(currencyOnDeath);
        WaveSpawner.instance.OnEnemyDied();
        Destroy(gameObject);
    }
    
    void ShowFloatingText(string text)
    {
        if (floatingTextPrefab)
        {
            Vector3 spawnPos = transform.position + new Vector3(0, 2f, 0);
            GameObject ft = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
            FloatingText floatingText = ft.GetComponent<FloatingText>();
            if (floatingText != null)
            {
                floatingText.Initialize(text, Color.red);
            }
        }
    }
}

