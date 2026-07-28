using UnityEngine;

public enum SoundType
{
    LASER,
    DAMAGE,
    EXPLOSION,
    DEATH,
    TUTORIAL
}

[System.Serializable]
public class SoundEntry
{
    public SoundType sound;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundEntry[] sounds;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField, Range(0f, 1f)] private float gameplayMusicVolume = 1f;
    [SerializeField, Min(0f)] private float explosionReplayDelay = 0.08f;

    private static SoundManager instance;
    private AudioSource sfxAudioSource;
    private AudioSource musicAudioSource;
    private float lastExplosionPlayTime = float.NegativeInfinity;

    private void Awake()
    {
        instance = this;

        AudioSource[] audioSources = GetComponents<AudioSource>();
        sfxAudioSource = audioSources.Length > 0 ? audioSources[0] : gameObject.AddComponent<AudioSource>();

        if (audioSources.Length > 1)
        {
            musicAudioSource = audioSources[1];
        }
        else
        {
            musicAudioSource = gameObject.AddComponent<AudioSource>();
        }

        musicAudioSource.playOnAwake = false;
        musicAudioSource.loop = true;
        musicAudioSource.spatialBlend = 0f;
    }

    private void Start()
    {
        PlayGameplayMusic();
    }

    private void Update()
    {
        RefreshGameplayMusicVolume();
    }

    public static void PlaySound(SoundType sound)
    {
        if (instance == null || instance.sfxAudioSource == null) return;
        if (instance.ShouldSkipSound(sound)) return;

        foreach (SoundEntry entry in instance.sounds)
        {
            if (entry.sound == sound && entry.clip != null)
            {
                instance.sfxAudioSource.PlayOneShot(entry.clip, entry.volume);
                return;
            }
        }
    }

    public static void PlaySound(SoundType sound, float volumeMultiplier)
    {
        if (instance == null || instance.sfxAudioSource == null) return;
        if (instance.ShouldSkipSound(sound)) return;

        foreach (SoundEntry entry in instance.sounds)
        {
            if (entry.sound == sound && entry.clip != null)
            {
                float finalVolume = Mathf.Clamp01(entry.volume * volumeMultiplier);
                instance.sfxAudioSource.PlayOneShot(entry.clip, finalVolume);
                return;
            }
        }
    }

    private void PlayGameplayMusic()
    {
        if (musicAudioSource == null || gameplayMusic == null)
        {
            return;
        }

        musicAudioSource.clip = gameplayMusic;
        RefreshGameplayMusicVolume();
        musicAudioSource.Play();
    }

    private void RefreshGameplayMusicVolume()
    {
        if (musicAudioSource == null)
        {
            return;
        }

        musicAudioSource.mute = AudioSettingsPrefs.IsMuted();
        musicAudioSource.volume = gameplayMusicVolume
            * AudioSettingsPrefs.GetSavedVolumeNormalized(AudioSettingsPrefs.MasterVolumeKey)
            * AudioSettingsPrefs.GetSavedVolumeNormalized(
                AudioSettingsPrefs.GameplayMusicVolumeKey,
                fallbackToLegacyMusic: true);
    }

    private bool ShouldSkipSound(SoundType sound)
    {
        if (sound != SoundType.EXPLOSION || explosionReplayDelay <= 0f)
        {
            return false;
        }

        if (Time.unscaledTime - lastExplosionPlayTime < explosionReplayDelay)
        {
            return true;
        }

        lastExplosionPlayTime = Time.unscaledTime;
        return false;
    }
}
