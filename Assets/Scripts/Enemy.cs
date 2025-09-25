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

    // A reference to the component that displays the sprite
    private SpriteRenderer spriteRenderer;

    [Header("Setup")]
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private Coroutine freezeCoroutine;
    
    void Awake()
    {
        // Get the SpriteRenderer component when the enemy is created
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // --- NEW DEBUG CHECK ---
        // This will immediately tell you if the sprite component is missing.
        if (spriteRenderer == null)
        {
            Debug.LogError("FATAL ERROR: No SpriteRenderer found on the enemy prefab or its children! Flipping will not work.", this);
        }
    }

    /// <summary>
    /// This is called by the WaveSpawner to
    /// give the enemy its specific stats from an EnemyData asset.
    /// </summary>
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

            // --- FLIPPING LOGIC ---
            if (spriteRenderer != null)
            {
                // If the enemy is moving horizontally, update its flip state.
                if (Mathf.Abs(direction.x) > 0.01f) // Use Abs to check for any horizontal movement
                {
                    spriteRenderer.flipX = direction.x > 0; // Flip if moving left (x is negative)
                }
                // If moving perfectly vertically, the flip state remains unchanged from the last
                // horizontal movement, which looks more natural.
            }
            // --- END FLIPPING LOGIC ---

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

    void Die()
    {
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

            // This assumes you have a FloatingText script with an Initialize method
            ft.GetComponent<FloatingText>().Initialize(text, Color.red);
        }
    }

    public void TakeDamage(float damage)
    {
        SoundManager.instance.PlayEnemyHitSound();
        health -= damage;
         UIManager.Instance.AddFuel(damage);

        if (damage > 1.0f)
        {
            ShowFloatingText(damage.ToString());

            if (freezeCoroutine != null)
                StopCoroutine(freezeCoroutine);

            freezeCoroutine = StartCoroutine(FreezeCoroutine());
        }

        if (health <= 0)
        {
            SoundManager.instance.PlayEnemyDeathSound();
            Die();
        }
    }

    IEnumerator FreezeCoroutine()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(0.8f, 0f, 0f);
        }
        moveSpeed = 0;
        yield return new WaitForSeconds(freezTime);
        moveSpeed = initialspeed;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        freezeCoroutine = null;
    }
}

