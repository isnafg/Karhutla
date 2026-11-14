using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    public AudioClip musicClip;

    [Header("Sound effects")]
    public AudioClip pickupClip;
    public AudioClip fireHitClip;
    public AudioClip obstacleHitClip;
    public AudioClip gameOverClip;
    public AudioClip resumeClip;
    public AudioClip clickClip;
    public AudioClip warningClip;
    public AudioClip gunshotClip;
    public AudioClip bulletHitClip;

    [Header("Fire ambience")]
    public AudioClip fireLoopClip;

    [Header("Volumes")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 0.8f;
    [Range(0f, 1f)] public float fireVolume = 0.5f;

    [Header("Timing")]
    public float gameOverDelay = 0.8f;

    AudioSource musicSource;
    AudioSource sfxSource;
    AudioSource fireSource;
    AudioSource warningSource;
    AudioSource gameOverSource;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;
        musicSource.ignoreListenerPause = true;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;

        fireSource = gameObject.AddComponent<AudioSource>();
        fireSource.loop = true;
        fireSource.playOnAwake = false;
        fireSource.volume = fireVolume;

        warningSource = gameObject.AddComponent<AudioSource>();
        warningSource.playOnAwake = false;
        warningSource.volume = sfxVolume;

        gameOverSource = gameObject.AddComponent<AudioSource>();
        gameOverSource.playOnAwake = false;
        gameOverSource.volume = sfxVolume;
    }

    void Start()
    {
        if (musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.Play();
        }
    }

    void Play(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    public void PlayPickup() { Play(pickupClip); }
    public void PlayFireHit() { Play(fireHitClip); }
    public void PlayObstacleHit() { Play(obstacleHitClip); }
    public void PlayResume() { Play(resumeClip); }
    public void PlayClick() { Play(clickClip); }
    public void PlayGunshot() { Play(gunshotClip); }
    public void PlayBulletHit() { Play(bulletHitClip); }

    // The game over sound starts a moment later, so the hit sound is heard first
    public void PlayGameOver()
    {
        if (gameOverClip != null)
        {
            gameOverSource.clip = gameOverClip;
            gameOverSource.PlayDelayed(gameOverDelay);
        }
    }

    // The warning sound has its own source, so it can be cut off when the marks disappear
    public void PlayWarning()
    {
        if (warningClip != null)
        {
            warningSource.clip = warningClip;
            warningSource.Play();
        }
    }

    public void StopWarning()
    {
        if (warningSource != null && warningSource.isPlaying)
        {
            warningSource.Stop();
        }
    }

    public void SetFireAmbience(bool on)
    {
        if (fireLoopClip == null) { return; }
        if (on && !fireSource.isPlaying)
        {
            fireSource.clip = fireLoopClip;
            fireSource.Play();
        }
        else if (!on && fireSource.isPlaying)
        {
            fireSource.Stop();
        }
    }
}