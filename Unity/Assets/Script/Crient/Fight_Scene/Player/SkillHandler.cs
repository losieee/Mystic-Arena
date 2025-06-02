using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SkillHandler : MonoBehaviour
{
    public SkillData skillData;

    [Header("UI")]
    public Image cooldownImage;
    public TextMeshProUGUI cooldownText;
    public GameObject glowEffect;

    [Header("이펙트 프리팹")]
    public GameObject skillEffectPrefab;
    public GameObject shiftEffectPrefab;

    [Header("기타")]
    public UnityEngine.Events.UnityEvent onSkillUsed;

    public Fight_Demo knight_Move;

    private AudioSource audioSource;
    private Animator animator;

    private bool isCooldown = false;
    private bool isCasting = false;
    public bool IsCasting => isCasting;
    public bool IsUsingSkill => isCooldown;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        animator = GetComponent<Animator>();
        audioSource.playOnAwake = false;
    }

    public void TryUseSkill()
    {
        if (isCooldown || knight_Move == null || knight_Move.IsAttacking())
            return;

        if (skillData.skillType == SkillType.Q || skillData.skillType == SkillType.Shift)
        {
            isCasting = true;
            knight_Move.DontMove();
        }

        switch (skillData.skillType)
        {
            case SkillType.Q:
                RotateTowardsMouse();
                knight_Move.animator.SetTrigger("Qskill");
                break;

            case SkillType.E:
                if (skillData.skillSound != null)
                    audioSource.PlayOneShot(skillData.skillSound);
                StartCoroutine(knight_Move.SpeedBoost(3f, 15f));
                break;

            case SkillType.Shift:
                knight_Move.animator.SetTrigger("isDashing");

                if (skillData.skillSound != null)
                    audioSource.PlayOneShot(skillData.skillSound);
                
                // 대시 위치/방향 정보 계산 후 이펙트 생성
                Vector3 dashStart = knight_Move.transform.position;
                Vector3 dashDir = knight_Move.transform.forward;
                float dashLength = skillData.effectSpawnDistance;

                SpawnShiftEffect(dashStart, dashDir, dashLength);
                knight_Move.StartCoroutine(knight_Move.DashForward());
                break;
        }

        StartCoroutine(CooldownRoutine());
    }

    /// 방향 기반 Q스킬 회전
    private void RotateTowardsMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Field")))
        {
            Vector3 direction = hit.point - knight_Move.transform.position;
            direction.y = 0;
            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                knight_Move.transform.rotation = lookRotation;
            }
        }
    }

    /// Q스킬 효과 생성
    public void SpawnSkillEffect()
    {
        if (skillData.skillEffectPrefab == null || knight_Move == null)
            return;

        Transform swordTransform = knight_Move.swordTransform;

        GameObject effect = Instantiate(
            skillData.skillEffectPrefab,
            swordTransform.position,
            swordTransform.rotation,
            swordTransform
        );

        Destroy(effect, skillData.effectDuration);
    }

    /// 대시 이펙트 생성
    public void SpawnShiftEffect(Vector3 startPos, Vector3 direction, float length)
    {
        if (skillData.trailEffectPrefab == null)
            return;

        Vector3 spawnPosition = startPos + direction.normalized * (length / 2f);
        spawnPosition.y = knight_Move.swordTransform.position.y;

        Quaternion spawnRotation = Quaternion.LookRotation(-direction);

        GameObject effect = Instantiate(skillData.trailEffectPrefab, spawnPosition, spawnRotation);

        effect.transform.localScale = new Vector3(1f, 1f, length);
        Destroy(effect, skillData.effectDuration);
    }

    /// Q스킬 사운드
    public void QSkillSound()
    {
        if (skillData.skillSound != null)
            audioSource.PlayOneShot(skillData.skillSound);
    }

    /// 쿨다운 처리
    private IEnumerator CooldownRoutine()
    {
        onSkillUsed?.Invoke();
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

        glowEffect.SetActive(true);

        CanvasGroup canvasGroup = glowEffect.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = glowEffect.AddComponent<CanvasGroup>();

        float fadeInTime = 0.1f;
        for (float t = 0; t < fadeInTime; t += Time.deltaTime)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeInTime);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(0.1f);

        float fadeOutTime = 0.1f;
        for (float t = 0; t < fadeOutTime; t += Time.deltaTime)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeOutTime);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        glowEffect.SetActive(false);
    }
    public void EndSkillCast()
    {
        isCasting = false;
    }
}
