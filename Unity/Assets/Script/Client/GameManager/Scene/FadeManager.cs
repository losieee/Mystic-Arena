using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    [Header("Fade 설정")]
    public Image fadeImage;
    public float fadeDuration = 0.5f;

    // 허용된 씬 목록
    private readonly HashSet<string> allowedScenes = new HashSet<string>
    {
        "Stage_1", "Stage_2", "Stage_3", "Stage_4", "Stage_5", "Stage_6", "Stage_7", "Stage_8", "Stage_9", "BossIntro"
    };

    private void Awake()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (!allowedScenes.Contains(currentScene))
        {
            Debug.Log($"[FadeManager] 현재 씬({currentScene})은 허용되지 않아서 FadeManager 오브젝트를 파괴합니다.");
            Destroy(gameObject);
            return;
        }

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            StartCoroutine(FadeOut());
        }
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeAndSwitchScene(sceneName));
    }

    private IEnumerator FadeAndSwitchScene(string sceneName)
    {
        yield return StartCoroutine(FadeIn());
        SceneManager.LoadScene(sceneName);
        yield return null;
        yield return StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        if (fadeImage == null) yield break;

        fadeImage.raycastTarget = true;
        float time = 0f;
        Color color = fadeImage.color;
        while (time < fadeDuration)
        {
            color.a = Mathf.Lerp(0f, 1f, time / fadeDuration);
            fadeImage.color = color;
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;
    }

    private IEnumerator FadeOut()
    {
        if (fadeImage == null) yield break;
        yield return new WaitForSecondsRealtime(1f);
        float time = 0f;
        Color color = fadeImage.color;
        while (time < fadeDuration)
        {
            color.a = Mathf.Lerp(1f, 0f, time / fadeDuration);
            fadeImage.color = color;
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;
        fadeImage.raycastTarget = false;
    }
}
