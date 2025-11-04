using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip backgroundMusic;

    void Start()
    {
        if (musicSource && backgroundMusic)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.playOnAwake = false; // we’ll call Play() ourselves
            musicSource.volume = 1f;
            musicSource.spatialBlend = 0f;   // 2D
            musicSource.Play();
        }
    }

    public void PlaySfx(AudioClip clip, float vol = 1f)
    {
        if (sfxSource && clip) sfxSource.PlayOneShot(clip, vol);
    }
}
