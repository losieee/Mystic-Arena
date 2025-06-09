using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class ButtonHoverSound : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public AudioClip hoverSound;  // 마우스 올렸을 때 나는 소리
    private AudioSource audioSource;

    public GameObject text;
    public GameObject option;
    public AudioClip clickSound;
    public Image fadeImage;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (option.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseOption();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.SetActive(true);
        if (hoverSound != null && audioSource != null)
        {
            audioSource.clip = hoverSound;
            audioSource.volume = 0.1f;
            audioSource.Play();
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        text.SetActive(false);
    }

    public void StartGame()
    {
        StartCoroutine(FadeAndLoadScene("CutScene"));
    }

    public void OnSetting()
    {
        option.SetActive(true);
    }
    public void CloseOption()
    {
        option.SetActive(false);
    }
    public void ExitGame()
    {

    }
    public void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.clip = clickSound;
            audioSource.volume = 0.3f;
            audioSource.Play();
        }
    }
    public IEnumerator FadeAndLoadScene(string sceneName)
    {
        fadeImage.gameObject.SetActive(true);
        float duration = 1f;
        float timer = 0f;

        Color color = fadeImage.color;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / duration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(sceneName);
    }
}
