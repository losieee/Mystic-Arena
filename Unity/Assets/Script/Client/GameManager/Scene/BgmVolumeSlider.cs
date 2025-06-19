using UnityEngine;
using UnityEngine.UI;

public class BgmVolumeSlider : MonoBehaviour
{
    public Slider slider;               
    public AudioSource bgmSource;       

    private void Start()
    {
        if (bgmSource != null && slider != null)
        {
            slider.value = bgmSource.volume;
            slider.onValueChanged.AddListener(SetBgmVolume);
        }
    }

    public void SetBgmVolume(float value)
    {
        if (bgmSource != null)
        {
            bgmSource.volume = value;
        }
    }
}
