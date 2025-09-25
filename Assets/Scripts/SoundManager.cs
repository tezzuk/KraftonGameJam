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
    public AudioClip buttonClickSound; // <-- NEW: The sound for UI button clicks
    public AudioClip threadConnectSound;

    [Header("Music Clips")]
    public AudioClip buildPhaseMusic;
    public AudioClip defensePhaseMusic;
    public AudioClip rewindSound;
    public AudioClip crystalDamageSound;
    public AudioClip flamethrowerLoopSound;
    public AudioSource loopingSfxSource;
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
    public void PlayCrystalDamageSound()
    {
        if (crystalDamageSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(crystalDamageSound);
        }
    }

    public void PlayEnemyHitSound()
    {
        if (enemyHitSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(enemyHitSound, 3.0f);
        }
    }

    public void PlayEnemyDeathSound()
    {
        if (enemyDeathSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(enemyDeathSound, 2.0f);
        }
    }

    public void PlayTowerPlaceSound()
    {
        if (towerPlaceSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(towerPlaceSound);
        }
    }
    public void PlayRewindSound()
    {
        if (rewindSound != null && sfxSource != null)
        {
            // --- THE CHANGE ---
            // The second number (1.5f) is a volume multiplier.
            // 1.0f is normal volume, 1.5f is 50% louder.
            sfxSource.PlayOneShot(rewindSound, 1.5f);
        }
    }

    public void PlayButtonClickSound()
    {
        if (buttonClickSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(buttonClickSound);
        }
    }

    // --- Music Control Methods ---

    public void PlayBuildMusic()
    {
        if (musicSource != null && buildPhaseMusic != null)
        {
            if (musicSource.clip == buildPhaseMusic && musicSource.isPlaying) return;

            musicSource.clip = buildPhaseMusic;
            musicSource.Play();
        }
    }

    public void PlayDefenseMusic()
    {
        if (musicSource != null && defensePhaseMusic != null)
        {
            if (musicSource.clip == defensePhaseMusic && musicSource.isPlaying) return;

            musicSource.clip = defensePhaseMusic;
            musicSource.Play();
        }
    }
    public void PlayThreadConnectSound()
    {
        if (threadConnectSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(threadConnectSound);
        }
    }
 public void PlayFlamethrowerSound()
    {
        if (loopingSfxSource != null && flamethrowerLoopSound != null)
        {
            // Assign the clip and play it only if it's not already playing
            if (loopingSfxSource.clip == flamethrowerLoopSound && loopingSfxSource.isPlaying) return;
            
            loopingSfxSource.clip = flamethrowerLoopSound;
            loopingSfxSource.Play();
        }
    }

    public void StopFlamethrowerSound()
    {
        if (loopingSfxSource != null && loopingSfxSource.isPlaying)
        {
            loopingSfxSource.Stop();
        }
    }


}

