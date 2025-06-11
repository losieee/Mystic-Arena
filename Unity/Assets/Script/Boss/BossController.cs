using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.ProjectWindowCallback;

public class BossController : MonoBehaviour
{
    public EnemySO enemySO;
    public AudioClip backGroundMusic;

    // º¸½º »ç¸Á ÈÄ
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 3f;
    public static bool isBossDead = false;
    public Image hpBarImage;

    private bool isDead = false;
    private bool isFadeStarted = false;


    void Start()
    {
        if (backGroundMusic != null)
        {
            GameObject audioObj = new GameObject("TempAudio_BackSound");
            audioObj.transform.position = transform.position;

            AudioSource audioSource = audioObj.AddComponent<AudioSource>();
            audioSource.clip = backGroundMusic;
            audioSource.volume = 0.01f;
            audioSource.loop = false;

            audioSource.Play();

            Destroy(audioObj, backGroundMusic.length);
        }
        enemySO.monstercurrHp = enemySO.monsterHp;
        isDead = false;
        isFadeStarted = false;

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.gameObject.SetActive(false);
        }
        if (hpBarImage != null)
        {
            hpBarImage.fillAmount = 0f;
            StartCoroutine(AnimateHPBarFill());
        }
    }
    private IEnumerator AnimateHPBarFill()
    {
        float fillDuration = 1f;
        float elapsed = 0f;

        while (elapsed < fillDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / fillDuration);
            hpBarImage.fillAmount = progress;

            yield return null;
        }

        hpBarImage.fillAmount = 1f;
    }
    public void TakeDamage(float damage)
    {
        enemySO.monstercurrHp -= damage;
        enemySO.monstercurrHp = Mathf.Clamp(enemySO.monstercurrHp, 0, enemySO.monsterHp);
        UpdateHPUI();
    }
    private void UpdateHPUI()
    {
        if (hpBarImage != null)
        {
            hpBarImage.fillAmount = enemySO.monstercurrHp / enemySO.monsterHp;
        }
    }

    private void Update()
    {
        if (enemySO.monstercurrHp <= 0 && !isDead)
        {
            isDead = true;
            isBossDead = true;

            if (!isFadeStarted)
            {
                isFadeStarted = true;
                StartCoroutine(FadeOutAndLoadScene());
            }
        }
    }

    private IEnumerator FadeOutAndLoadScene()
    {
        float elapsed = 0f;

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.gameObject.SetActive(true);

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Clamp01(elapsed / fadeDuration);
                fadeCanvasGroup.alpha = alpha;

                yield return null;
            }
            SceneManager.LoadScene("BossCut");
        }
    }
}
