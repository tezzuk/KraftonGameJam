using UnityEngine;
using System.Collections;

// This script handles the arcing movement and area-of-effect (AOE) explosion.
public class MortarShell : MonoBehaviour
{
    [Header("Stats")]
    public int damage = 50; // High damage
    public float areaOfEffectRadius = 3f;
    public float travelTime = 1.5f;
    public float arcHeight = 5f;

    private Transform target;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        StartCoroutine(ArcToTarget());
    }

    IEnumerator ArcToTarget()
    {
        Vector3 startPosition = transform.position;
        // Lock the target's position at the moment of firing for accuracy.
        Vector3 targetPosition = target.position; 
        float timer = 0f;

        while (timer < travelTime)
        {
            float t = timer / travelTime;
            
            // Calculate the position on a straight line between start and target
            Vector3 position = Vector3.Lerp(startPosition, targetPosition, t);
            
            // Add the arc height using a sine wave to create a smooth curve
            position.y += arcHeight * Mathf.Sin(t * Mathf.PI);

            transform.position = position;

            timer += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        
        // Ensure we land at the exact target position and explode.
        transform.position = targetPosition;
        Explode();
    }

    void Explode()
    {
        // Find all colliders within the explosion radius.
        // It's best practice to create an "Enemies" layer and specify it here.
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, areaOfEffectRadius);
        
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            // If the object has an Enemy script, deal damage to it.
            if (enemyCollider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(damage);
            }
        }
        
        // You would spawn an explosion visual effect here.
        Destroy(gameObject);
    }
    
    // Helper to see the explosion radius in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, areaOfEffectRadius);
    }
}

