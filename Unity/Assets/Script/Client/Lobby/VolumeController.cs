using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class VolumeController : MonoBehaviour
{
    public static float masterVolume = 1f;
    public Slider volumeSlider;
    public List<AudioSource> targetAudioSources;
    public AudioSource bgmAudioSource;

    void Start()
    {
        float initVolume = volumeSlider.value;
        SetVolume(initVolume);
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float volume)
    {
        masterVolume = volume;

        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = Mathf.Clamp01(volume * 2f);
        }

        foreach (var audioSource in targetAudioSources)
        {
            if (audioSource != null)
                audioSource.volume = volume;
        }
    }
}
