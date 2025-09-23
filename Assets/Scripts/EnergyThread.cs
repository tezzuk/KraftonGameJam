using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EnergyThread : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float scrollSpeed = 1f; // How fast the energy flows

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (startPoint == null || endPoint == null)
        {
            // If a connection is broken, destroy the thread
            Destroy(gameObject);
            return;
        }
        
        // Update the start and end points of the line
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, endPoint.position);

        // --- The Animation Logic ---
        // Scroll the texture's offset over time
        float textureOffset = -Time.time * scrollSpeed;
        lineRenderer.material.mainTextureOffset = new Vector2(textureOffset, 0f);
    }
}
