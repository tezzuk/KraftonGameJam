using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float duration = 2f;
    private float timer;
    private TextMeshPro textMesh;
    private Color startColor;

    public void Initialize(string text, Color color)
    {
        textMesh = GetComponent<TextMeshPro>();
        textMesh.text = text;
        textMesh.color = color;
        startColor = color;
        timer = 0f;
    }

    void Update()
    {
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
