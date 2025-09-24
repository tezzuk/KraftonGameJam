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

    //public UIManager uiManager;

    [Header("Setup")]
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private Coroutine freezeCoroutine;
    

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
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

    }

    void Update()
    {
        // This part remains the same, but the 'else' block is removed.
        // The enemy will now stop moving when it reaches the final waypoint.
        if (waypoints != null && currentWaypointIndex < waypoints.Length)
        {
            Transform targetWaypoint = waypoints[currentWaypointIndex];
            Vector3 direction = (targetWaypoint.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
            {
                currentWaypointIndex++;
            }
        }
    }

    // This new method handles the collision with the crystal.
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object we collided with is tagged as "Crystal".
        if (other.CompareTag("Crystal"))
        {
            // Tell the GameManager to reduce health and then destroy this enemy.
            GameManager.instance.EnemyReachedCrystal();
            Destroy(gameObject);
        }
    }

    void Die()
    {
        GameManager.instance.AddCurrency(currencyOnDeath);
        // Tell the WaveSpawner that this enemy has died so it can track wave progress
        WaveSpawner.instance.OnEnemyDied();
        Destroy(gameObject);
    }
    void ShowFloatingText(string text)
    {
        if (floatingTextPrefab)
        {
            // Spawn above enemy
            Vector3 spawnPos = transform.position + new Vector3(0, 2f, 0);
            GameObject ft = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);

            // Initialize
            ft.GetComponent<FloatingText>().Initialize(text, Color.red);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        UIManager.Instance.AddFuel(damage);

        if (damage > 1.0f)
        {
            ShowFloatingText("-" + damage.ToString());

            // Restart freeze effect safely
            if (freezeCoroutine != null)
                StopCoroutine(freezeCoroutine);

            freezeCoroutine = StartCoroutine(FreezeCoroutine()); // Fixed typo
        }

        if (health <= 0)
        {
            Die();
        }
    }

    IEnumerator FreezeCoroutine() // Fixed typo
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        // Freeze
        sr.color = new Color(0.5f, 0.2f, 1f);
        moveSpeed = 0;
        yield return new WaitForSeconds(freezTime);
        // Unfreeze
        moveSpeed = initialspeed;
        sr.color = originalColor;

        freezeCoroutine = null;
    }

}

