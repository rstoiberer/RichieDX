using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;
    public AudioClip backgroundMusic;
    public AudioClip stumbleSound;

    private void Start()
    {
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    public void PlaySXF(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
    
}
