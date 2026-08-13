using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioSource backgroundMusic;
    public AudioSource sfxSource;

    public AudioClip successSound;
    public AudioClip errorSound;
    public AudioClip powerRestoreSound;
    public AudioClip doorOpenSound;
    public AudioClip alarmSound;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}
