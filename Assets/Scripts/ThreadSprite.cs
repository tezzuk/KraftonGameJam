using UnityEngine;

public class ThreadSprite : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float thickness = 0.1f; // The thickness of the cable

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (startPoint == null || endPoint == null) return;
        
        // Find the direction and distance between points
        Vector2 direction = endPoint.position - startPoint.position;
        float distance = direction.magnitude;

        // Position the sprite at the midpoint
        transform.position = startPoint.position + (Vector3)direction / 2;
        
        // Rotate the sprite to face the endpoint
        transform.right = direction; // Points the 'right' side of the sprite along the direction

        // Scale the sprite to stretch between the points
        // This assumes your sprite is a simple horizontal line or capsule
        if (spriteRenderer != null)
        {
            spriteRenderer.size = new Vector2(distance, thickness);
        }
    }
}