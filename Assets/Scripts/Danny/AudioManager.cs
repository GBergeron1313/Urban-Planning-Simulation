using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source ")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    [Header("Audio Clip ")]
    public AudioClip background;
    public AudioClip placedown;
    public AudioClip buttonclick;

    private void Start()
    {
        /*musicSource.clip = background;*/
        /*musicSource.Play();*/
    }

    public void PlaySFX(AudioClip clip, float volume_scale = 1f)
    {
        SFXSource.PlayOneShot(clip, volume_scale);
    }
}
