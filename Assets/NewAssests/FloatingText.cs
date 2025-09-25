using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float duration = 2f;
    private float timer;
    private TextMeshPro textMesh;
    private Color startColor;
    private bool isInitialized = false; // A flag to prevent Update from running too early

    // Get the component as soon as the object is created
    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh == null)
        {
            Debug.LogError("FloatingText prefab is missing a TextMeshPro component!", this);
            Destroy(gameObject); // Destroy if not set up correctly
        }
    }

    public void Initialize(string text, Color color)
    {
        textMesh.text = text;
        textMesh.color = color;
        startColor = color;
        timer = 0f;
        isInitialized = true; // Set the flag to true
    }

    void Update()
    {
        // Don't run the logic until Initialize has been called
        if (!isInitialized) return;

        // Move upward
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // Fade out
        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, timer / duration);
        textMesh.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

        // Destroy after duration
        if (timer >= duration)
            Destroy(gameObject);
    }
}
