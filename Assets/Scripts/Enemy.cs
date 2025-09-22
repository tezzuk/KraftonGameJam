using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform[] waypoints;  // Assign in inspector or dynamically
    public float moveSpeed = 2f;
    private int currentWaypointIndex = 0;

    public int health = 10;  // Optional for later tower attacks

    void Update()
    {
        if (currentWaypointIndex < waypoints.Length)
        {
            // Move towards current waypoint
            Transform targetWaypoint = waypoints[currentWaypointIndex];
            Vector3 direction = (targetWaypoint.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            // Optional: rotate enemy to face movement direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);

            // Check if reached waypoint
            if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
            {
                currentWaypointIndex++;
            }
        }
        else
        {
            ReachGoal();
        }
    }

    void ReachGoal()
    {
        // Enemy reached the town
        Debug.Log("Enemy reached the town!");
        Destroy(gameObject); // Remove enemy
        // Optional: reduce town health
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
