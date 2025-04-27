using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Kino;

public class LobbyManager : MonoBehaviour
{
    public AudioClip clickSound;     // 새로 추가: 버튼 클릭 효과음
    public AudioClip glitchSound;    // 원래 있던 글리치 효과음
    private AudioSource audioSource;

    public DigitalGlitch glitchEffect;
    public AnalogGlitch analogglitch;
    public float clickSoundVolume = 0.5f;

    private bool isClicked = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;

        if (glitchEffect != null)
        {
            glitchEffect.enabled = false;
            analogglitch.enabled = false;
        }
    }


    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        if (isClicked) return;
        isClicked = true;

        glitchEffect.intensity = 0f;
        analogglitch.scanLineJitter = 0f;
        analogglitch.verticalJump = 0f;
        analogglitch.horizontalShake = 0f;
        analogglitch.colorDrift = 0f;

        StartCoroutine(DelayedGlitchAndSound());
    }

    public void Tutorial()
    {
        if (isClicked) return;
        isClicked = true;

        // 1. 튜토리얼 버튼 클릭하자마자 버튼 효과음 재생
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound, clickSoundVolume);
        }


        glitchEffect.intensity = 0f;
        analogglitch.scanLineJitter = 0f;
        analogglitch.verticalJump = 0f;
        analogglitch.horizontalShake = 0f;
        analogglitch.colorDrift = 0f;

        StartCoroutine(ToTutorialGlitchAndSound());
    }

    private IEnumerator ToTutorialGlitchAndSound()
    {
        yield return new WaitForSeconds(1f); // 1초 대기

        // 2. 글리치 이펙트 켜기
        if (glitchEffect != null && analogglitch != null)
        {
            glitchEffect.enabled = true;
            analogglitch.enabled = true;
            StartCoroutine(IncreaseGlitch());
        }

        // 3. 글리치 효과음 재생
        if (glitchSound != null)
        {
            audioSource.PlayOneShot(glitchSound);
            yield return new WaitForSeconds(glitchSound.length);
        }

        SceneManager.LoadScene("Tutorial");
    }

    private IEnumerator DelayedGlitchAndSound()
    {
        yield return new WaitForSeconds(1f); // 1초 대기

        if (glitchEffect != null && analogglitch != null)
        {
            glitchEffect.enabled = true;
            analogglitch.enabled = true;
            StartCoroutine(IncreaseGlitch());
        }

        if (glitchSound != null)
        {
            audioSource.PlayOneShot(glitchSound);
            yield return new WaitForSeconds(glitchSound.length);
        }

        SceneManager.LoadScene("Character_Choice");
    }

    private IEnumerator IncreaseGlitch()
    {
        float duration = glitchSound != null ? glitchSound.length : 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            analogglitch.scanLineJitter = Mathf.Lerp(0.1f, 1.0f, t);
            analogglitch.verticalJump = Mathf.Lerp(0.05f, 0.5f, t);
            analogglitch.horizontalShake = Mathf.Lerp(0.1f, 0.8f, t);
            analogglitch.colorDrift = Mathf.Lerp(0.1f, 1.0f, t);
            glitchEffect.intensity = Mathf.Lerp(0.0f, 1.0f, t);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
