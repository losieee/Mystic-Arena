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

    public AudioClip skillSound; // 스킬 사운드
    public AudioClip dashSound;
    public AudioClip qKeySound;
    public AudioClip eKeySound;

    private Animator animator;  // 스킬 애니메이션

    public Fight_Demo knight_Move;

    private AudioSource audioSource;

    private bool isCooldown = false;
    private bool canUseSkill = true;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        animator = GetComponent<Animator>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        // Shift키를 눌렀을 때 특정 사운드 재생 (쿨타임 아닐 때만)
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashSound != null && !isCooldown)
        {
            audioSource.PlayOneShot(dashSound);
        }
    }
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

    public void TryUseSkill()
    {
        if (isCooldown || knight_Move == null || knight_Move.IsAttacking())
            return;

        knight_Move.DontMove();
        
        if (skillSound != null)
            audioSource.PlayOneShot(skillSound);

        switch (skillData.skillType)
        {
            case SkillType.Q:
                RotateTowardsMouse();
                knight_Move.animator.SetTrigger("Qskill");
                break;

            case SkillType.E:
                //knight_Move.animator.SetTrigger("Eskill"); 
                break;

            case SkillType.Shift:
                knight_Move.animator.SetTrigger("isDashing");
                knight_Move.StartCoroutine(knight_Move.DashForward()); // 대시 동작 직접 실행
                break;
        }

        StartCoroutine(CooldownRoutine());
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
}