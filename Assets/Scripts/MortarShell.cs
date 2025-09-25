using UnityEngine;
using System.Collections;

public class MortarShell : MonoBehaviour
{
    [Header("Stats")]
    public int damage; // Damage is now set by the Mortar tower
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
        Vector3 targetPosition = target.position;
        float timer = 0f;

        while (timer < travelTime)
        {
            float t = timer / travelTime;

            Vector3 position = Vector3.Lerp(startPosition, targetPosition, t);

            position.y += arcHeight * Mathf.Sin(t * Mathf.PI);

            transform.position = position;

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        Explode();
    }

    void Explode()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, areaOfEffectRadius);

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (enemyCollider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, areaOfEffectRadius);
    }
}
