using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Kino;
using UnityEditor;

public class LobbyManager : MonoBehaviour
{
    public AudioClip startSound;
    private AudioSource audioSource;

    public DigitalGlitch glitchEffect;
    public AnalogGlitch analogglitch;

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

    private IEnumerator DelayedGlitchAndSound()
    {
        yield return new WaitForSeconds(1f); // 1초 대기

        // 글리치 켜기
        if (glitchEffect != null && analogglitch != null)
        {
            glitchEffect.enabled = true;
            analogglitch.enabled = true;
            StartCoroutine(IncreaseGlitch());
        }

        // 효과음 재생
        if (startSound != null)
        {
            audioSource.PlayOneShot(startSound);
            yield return new WaitForSeconds(startSound.length);
        }

        SceneManager.LoadScene("Character_Choice");
    }

    private IEnumerator IncreaseGlitch()
    {
        float duration = startSound != null ? startSound.length : 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // Analog 글리치 점점 증가
            analogglitch.scanLineJitter = Mathf.Lerp(0.1f, 1.0f, t);
            analogglitch.verticalJump = Mathf.Lerp(0.05f, 0.5f, t);
            analogglitch.horizontalShake = Mathf.Lerp(0.1f, 0.8f, t);
            analogglitch.colorDrift = Mathf.Lerp(0.1f, 1.0f, t);

            // Digital 글리치 점점 증가
            glitchEffect.intensity = Mathf.Lerp(0.0f, 1.0f, t);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}