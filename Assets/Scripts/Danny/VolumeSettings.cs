using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlinder;
    [SerializeField] private Slider SFXSlinder;
    private void Start()
    {
        SetMusicVolume();
    }

    public void SetMusicVolume()
    {
        float volume = musicSlinder.value;
        myMixer.SetFloat("music",Mathf.Log10(volume)*20);
    }
    public void SetSFXVolume()
    {
        float volume = SFXSlinder.value;
        myMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
    }
}
