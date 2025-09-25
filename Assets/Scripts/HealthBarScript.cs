using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 10f;
    private float currentHealth;
    public GameManager gameManager;
    [Header("UI")]
    public Slider healthSlider;

    void Start()
    {
        currentHealth = maxHealth;

        // Setup slider
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    private void Update()
    {
        currentHealth = Mathf.Clamp(gameManager.crystalHealth, 0, maxHealth);
        healthSlider.value = currentHealth;
    }
}
