using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Kino;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;

    public AudioClip startSound;
    private AudioSource audioSource;
    
    public DigitalGlitch glitchEffect;
    public AnalogGlitch analogGlitch;

    private bool isClicked = false;

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 씬 전환 시 오브젝트 유지
        }
        else
        {
            Destroy(gameObject);  // 이미 싱글톤이 존재하면 자신을 삭제
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;

        if (glitchEffect != null) glitchEffect.enabled = false;
        if (analogGlitch != null) analogGlitch.enabled = false;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Main_Lobby와 Character_Lobby 씬에서만 유지하고 나머지 씬에서는 제거
        if (scene.name == "Main_Lobby" || scene.name == "Character_Lobby")
        {
            DontDestroyOnLoad(gameObject);  // 해당 씬에서만 유지
        }
        else
        {
            Destroy(gameObject);  // 다른 씬에서는 제거
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
        ResetGlitch();
        StartCoroutine(DelayedGlitchAndSound("Character_Choice"));
    }

    public void Tutorial()
    {
        if (isClicked) return;
        isClicked = true;
        ResetGlitch();
        StartCoroutine(DelayedGlitchAndSound("Tutorial"));
    }

    private void ResetGlitch()
    {
        if (glitchEffect != null) glitchEffect.intensity = 0f;
        if (analogGlitch != null)
        {
            analogGlitch.scanLineJitter = 0f;
            analogGlitch.verticalJump = 0f;
            analogGlitch.horizontalShake = 0f;
            analogGlitch.colorDrift = 0f;
        }
    }

    private IEnumerator DelayedGlitchAndSound(string sceneName)
    {
        yield return new WaitForSeconds(1f); // 1초 대기

        // 글리치 효과 켜기
        if (glitchEffect != null && analogGlitch != null)
        {
            glitchEffect.enabled = true;
            analogGlitch.enabled = true;
            StartCoroutine(IncreaseGlitch());
        }

        // 효과음 재생
        if (startSound != null)
        {
            audioSource.PlayOneShot(startSound);
            yield return new WaitForSeconds(startSound.length);
        }

        // 씬 전환
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator IncreaseGlitch()
    {
        float duration = startSound != null ? startSound.length : 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // Analog 글리치 점점 증가
            if (analogGlitch != null)
            {
                analogGlitch.scanLineJitter = Mathf.Lerp(0.1f, 1.0f, t);
                analogGlitch.verticalJump = Mathf.Lerp(0.05f, 0.5f, t);
                analogGlitch.horizontalShake = Mathf.Lerp(0.1f, 0.8f, t);
                analogGlitch.colorDrift = Mathf.Lerp(0.1f, 1.0f, t);
            }

            // Digital 글리치 점점 증가
            if (glitchEffect != null)
            {
                glitchEffect.intensity = Mathf.Lerp(0.0f, 1.0f, t);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}