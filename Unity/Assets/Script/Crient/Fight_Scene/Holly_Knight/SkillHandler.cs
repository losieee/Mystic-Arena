using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SkillHandler : MonoBehaviour
{
    public SkillData skillData;
    public Image cooldownImage;
    public TextMeshProUGUI cooldownText;
    public GameObject glowEffect;
    public UnityEngine.Events.UnityEvent onSkillUsed;

    private bool isCooldown = false;

    public void TryUseSkill()
    {
        if (!isCooldown && Input.GetKeyDown(skillData.activationKey))
        {
            StartCoroutine(CooldownRoutine());
        }
    }

    private IEnumerator CooldownRoutine()
    {
        onSkillUsed?.Invoke(); // 대시 등 동작 실행

        isCooldown = true;
        cooldownImage.gameObject.SetActive(true);
        cooldownText.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < skillData.cooldownTime)
        {
            elapsed += Time.deltaTime;
            float remaining = skillData.cooldownTime - elapsed;

            cooldownImage.fillAmount = 1 - (elapsed / skillData.cooldownTime);
            cooldownText.text = Mathf.CeilToInt(remaining).ToString();

            yield return null;
        }

        cooldownImage.gameObject.SetActive(false);
        cooldownText.gameObject.SetActive(false);
        isCooldown = false;

        if (glowEffect != null)
            StartCoroutine(ShowGlowBriefly());
    }

    private IEnumerator ShowGlowBriefly()
    {
        if (glowEffect == null) yield break;

        // 혹시 꺼져있다면 켜기
        glowEffect.SetActive(true);

        // CanvasGroup이 있으면 사용, 없으면 추가
        CanvasGroup canvasGroup = glowEffect.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = glowEffect.AddComponent<CanvasGroup>();

        // Fade In
        float fadeInTime = 0.1f;
        for (float t = 0; t < fadeInTime; t += Time.deltaTime)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeInTime);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // 잠깐 유지
        yield return new WaitForSeconds(0.1f);

        // Fade Out
        float fadeOutTime = 0.1f;
        for (float t = 0; t < fadeOutTime; t += Time.deltaTime)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeOutTime);
            yield return null;
        }
        canvasGroup.alpha = 0f;

        // 완전히 투명해졌으면 비활성화
        glowEffect.SetActive(false);
    }
}
