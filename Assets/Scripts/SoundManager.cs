using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // A singleton instance so we can access this from any script easily
    public static SoundManager instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource; // The component that will play sound effects

    [Header("Sound Clips")]
    public AudioClip enemyHitSound;
    public AudioClip enemyDeathSound; // The new sound clip for when an enemy dies

    void Awake()
    {
        // Set up the singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Plays the enemy hit sound effect.
    /// </summary>
    public void PlayEnemyHitSound()
    {
        if (enemyHitSound != null && sfxSource != null)
        {
            // PlayOneShot allows multiple hit sounds to overlap, which is perfect
            sfxSource.PlayOneShot(enemyHitSound);
        }
    }

    /// <summary>
    /// Plays the enemy death sound effect.
    /// </summary>
    public void PlayEnemyDeathSound()
    {
        if (enemyDeathSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(enemyDeathSound);
        }
    }
}

