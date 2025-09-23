using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Live Stats (Set by Spawner)")]
    public float health;
    public float moveSpeed;
    public int currencyOnDeath;
    public GameObject floatingTextPrefab;

    //public UIManager uiManager;

    [Header("Setup")]
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;

    /// <summary>
    /// This is called by the WaveSpawner to
    /// give the enemy its specific stats from an EnemyData asset.
    /// </summary>
    public void Setup(EnemyData data)
    {
        health = data.health;
        moveSpeed = data.moveSpeed;
        currencyOnDeath = data.currencyOnDeath;
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

    public void TakeDamage(float damage)
    {
        health -= damage;
        UIManager.Instance.AddFuel(damage);
        if (damage > 1.0f)
            ShowFloatingText("-" + damage.ToString());
        if (health <= 0)
        {
            Die();
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
}

