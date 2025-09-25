using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // A singleton instance so we can access this from any script easily
    public static SoundManager instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource; // The component that will play sound effects
    public AudioSource musicSource; // A separate source for background music

    [Header("Sound Clips")]
    public AudioClip enemyHitSound;
    public AudioClip enemyDeathSound;
    public AudioClip towerPlaceSound;

    [Header("Music Clips")]
    public AudioClip buildPhaseMusic;
    public AudioClip defensePhaseMusic;

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

    // --- Sound Effect Methods ---

    public void PlayEnemyHitSound()
    {
        if (enemyHitSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(enemyHitSound);
        }
    }

    public void PlayEnemyDeathSound()
    {
        if (enemyDeathSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(enemyDeathSound);
        }
    }

    public void PlayTowerPlaceSound()
    {
        if (towerPlaceSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(towerPlaceSound);
        }
    }

    // --- Music Control Methods ---

    /// <summary>
    /// Plays the music for the build phase.
    /// </summary>
    public void PlayBuildMusic()
    {
        if (musicSource != null && buildPhaseMusic != null)
        {
            // Check if the correct music is already playing to avoid restarting it
            if (musicSource.clip == buildPhaseMusic && musicSource.isPlaying) return;
            
            musicSource.clip = buildPhaseMusic;
            musicSource.Play();
        }
    }

    /// <summary>
    /// Plays the music for the defense phase.
    /// </summary>
    public void PlayDefenseMusic()
    {
        if (musicSource != null && defensePhaseMusic != null)
        {
            // Check if the correct music is already playing
            if (musicSource.clip == defensePhaseMusic && musicSource.isPlaying) return;

            musicSource.clip = defensePhaseMusic;
            musicSource.Play();
        }
    }
}

