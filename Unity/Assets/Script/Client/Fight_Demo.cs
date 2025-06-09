using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor.TerrainTools;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.ComponentModel;
using Unity.Properties;

public class Fight_Demo : MonoBehaviour
{
    public PlayerSO playerSO;

    [Header("Skill Handlers")]
    [SerializeField] private SkillHandler qSkillHandler;
    [SerializeField] private SkillHandler eSkillHandler;
    [SerializeField] private SkillHandler shiftSkillHandler;

    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashSpeed = 15f;

    [Range(0f, 1f)]
    public float attackSoundVolume = 0.2f;

    public Animator animator;
    public Transform swordTransform;
    public Transform swordObject;
    public AudioClip comboAttackSound;
    public AudioClip LastComboSound;
    public AudioClip attackSound;
    public SkillHandler skillHandler;
    public Image hpBarImage;
    public GameObject deathPanel;

    [HideInInspector]public bool canMove = true;
    [HideInInspector]public bool isWorking = false;
    private bool isMove = false;
    private bool isDashing = false;
    private bool isInvincible = false;
    public static bool isDead = false;
    private bool currentAttackHit = false;
    private bool isHit = false;
    private bool queuedAttackInput = false;

    // 콤보
    private int comboStep = 0;
    private bool isAttacking = false;
    private bool comboQueued = false;
    private bool canQueueNextCombo = false;

    private CapsuleCollider capsule;
    public NavMeshAgent agent;
    private Camera mainCamera;
    private Vector3 destination;
    private Coroutine comboResetCoroutine;
    private AudioSource audioSource;

    private Quaternion idleAttackRotationOffset;    //애니메이션 기울이기용

    private void Awake()
    {
        capsule = GetComponentInChildren<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        mainCamera = Camera.main;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Y축으로 30도 회전 오프셋
        idleAttackRotationOffset = Quaternion.Euler(0f, 30f, 0f);

        agent.updateRotation = false;
        agent.enabled = true;
        agent.acceleration = 1000f;
        agent.angularSpeed = 10000f;
        agent.stoppingDistance = 0.4f;
        agent.autoBraking = true;

        shiftSkillHandler.onSkillUsed.AddListener(() => StartCoroutine(DashForward()));
    }

    private void Start()
    {
        playerSO.playerCurrHp = playerSO.playerMaxHp;
        UpdateHPUI();
        Debug.Log(isDashing);

        isDead = false;
        canMove = true;
        isMove = false;
        isDashing = false;
        isInvincible = false;

        animator.SetBool("isDead", false);
        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible || isHit)
            return;

        playerSO.playerCurrHp -= damage;
        playerSO.playerCurrHp = Mathf.Clamp(playerSO.playerCurrHp, 0, playerSO.playerMaxHp);
        UpdateHPUI();

        if (playerSO.playerCurrHp <= 0 && !isDead)
        {
            isDead = true;
            animator.SetBool("isDead", true);
            agent.ResetPath();
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", false);

            if (deathPanel != null)
                deathPanel.SetActive(true);

            return;
        }

        StartCoroutine(PlayHitAnimation());
    }
    private IEnumerator PlayHitAnimation()
    {
        isHit = true;
        canMove = false;
        isAttacking = false;

        // 공격 취소
        comboStep = 0;
        comboQueued = false;
        canQueueNextCombo = false;
        animator.SetInteger("ComboCount", 0);
        animator.SetBool("isAttacking", false);

        animator.SetTrigger("Hit");

        // Hit 애니메이션 길이만큼 대기
        yield return new WaitForSeconds(0.16f);

        isHit = false;
        canMove = true;

        // 피격 끝난 후 공격 입력 있었으면 실행
        if (queuedAttackInput && !isDead)
        {
            queuedAttackInput = false;
            StartAttackCombo(); // 함수로 분리해서 사용
        }
    }
    private void UpdateHPUI()
    {
        if (hpBarImage != null)
        {
            hpBarImage.fillAmount = playerSO.playerCurrHp / playerSO.playerMaxHp;
        }
    }
    private void Update()
    {
        // 보스 사망 시
        if (BossController.isBossDead)
        {
            agent.ResetPath();
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", false);
            return;
        }

        // 플레이어 사망 시
        if (isDead || isHit)
        {
            agent.ResetPath();
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", false);
            return;
        }

        if (queuedAttackInput && !isHit && !isAttacking && canMove && !isDead)
        {
            queuedAttackInput = false;
            StartAttackCombo();
        }

        // 평소 조작 처리
        if (playerSO.playerCurrHp <= 0 && !isDead)
        {
            isDead = true;

            animator.SetBool("isDead", true);

            agent.ResetPath();
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", false);

            if (deathPanel != null)
            {
                deathPanel.SetActive(true);
            }
        }

        HandleComboInput();

        if (canMove && !isAttacking)
        {
            HandleKeyboardMoveInput();

            if (Input.GetKeyDown(KeyCode.Q))
                qSkillHandler.TryUseSkill();

            if (Input.GetKeyDown(KeyCode.E))
                eSkillHandler.TryUseSkill();

            if(Input.GetKeyDown(KeyCode.LeftShift))
                shiftSkillHandler.TryUseSkill();
        }

        LookMoveDirection();
    }
    private IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("BossRoom");
    }

    private void HandleKeyboardMoveInput()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        input = input.normalized;

        if (input != Vector3.zero)
        {
            Vector3 moveDir = Quaternion.Euler(0, mainCamera.transform.eulerAngles.y, 0) * input;
            agent.SetDestination(transform.position + moveDir);
            isMove = true;
        }
    }

    private void LookMoveDirection()
    {
        if (!isMove)
        {
            animator.SetBool("isRunning", false);
            return;
        }

        Vector3 moveDirection = agent.desiredVelocity.normalized;

        if (moveDirection != Vector3.zero)
        {
            moveDirection.y = 0;
            capsule.transform.rotation = Quaternion.LookRotation(moveDirection);
        }

        if (agent.enabled && agent.isOnNavMesh && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            isMove = false;
            agent.ResetPath();
            animator.SetBool("isRunning", false);
        }
        else
        {
            animator.SetBool("isRunning", true);
        }
    }

    private void HandleComboInput()
    {
        if (isWorking || isDead)
            return;

        // 입력 저장
        if (Input.GetMouseButtonDown(0))
        {
            // 피격 중이면 저장
            if (isHit)
            {
                queuedAttackInput = true;
                return;
            }

            if (canQueueNextCombo && comboStep < 3)
            {
                comboQueued = true;
                return;
            }

            if (isAttacking)
                return;

            StartAttackCombo(); // 바로 시작
        }
    }
    private void StartAttackCombo()
    {
        DontMove();

        // 마우스 위치 기준 회전
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Field")))
        {
            Vector3 dir = hit.point - transform.position;
            dir.y = 0;
            if (dir != Vector3.zero)
            {
                capsule.transform.rotation = Quaternion.LookRotation(dir) * idleAttackRotationOffset;
            }
        }

        comboStep = 1;
        isAttacking = true;
        currentAttackHit = false;
        animator.SetInteger("ComboCount", comboStep);
        animator.SetBool("isAttacking", true);
    }

    public void DontMove()
    {
        agent.ResetPath();
        isMove = false;
        canMove = false;
    }
    public void EndWorking()
    {
        isWorking = false;
        canMove = true;
    }

    public void EnableNextCombo()
    {
        canQueueNextCombo = true;
    }

    public void EndCombo()
    {
        if (comboResetCoroutine != null)
            StopCoroutine(comboResetCoroutine);

        comboResetCoroutine = StartCoroutine(DelayedComboReset());
    }

    private IEnumerator DelayedComboReset()
    {
        yield return new WaitForEndOfFrame();

        if (comboQueued && comboStep < 3)
        {
            comboStep++;
            animator.SetInteger("ComboCount", comboStep);
            comboQueued = false;
            canQueueNextCombo = false;
        }
        else
        {
            animator.SetInteger("ComboCount", 0);
            comboStep = 0;
            isAttacking = false;
            comboQueued = false;
            canQueueNextCombo = false;

            canMove = false;
            // 콤보 종료 후 대기
            yield return new WaitForSeconds(0.4f);
            canMove = true;
            animator.SetBool("isAttacking", false);
        }
    }

    public IEnumerator DashForward()
    {
        if (isAttacking)
        {
            // 대시 중에는 공격 상태를 해제
            isAttacking = false;
            comboQueued = false;
            canQueueNextCombo = false;
            comboStep = 0;
            animator.SetInteger("ComboCount", 0);
        }

        isDashing = true;
        isInvincible = true;
        animator.SetTrigger("isDashing");

        float dashTime = dashDistance / dashSpeed;
        Vector3 dashDirection = capsule.transform.forward;

        Vector3 start = transform.position;
        Vector3 end = start + dashDirection * dashDistance;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, dashDirection, out hit, dashDistance))
        {
            end = transform.position + dashDirection * (hit.distance - 0.3f);
        }

        SpawnDashTrail(start, start + dashDirection * dashDistance);

        float elapsed = 0f;
        while (elapsed < dashTime)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / dashTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        isDashing = false;
        isInvincible = false;
        shiftSkillHandler?.EndSkillCast();

        // 대시 후에는 움직임 가능
        canMove = true;
    }
    public void StopAgentImmediately()
    {
        if (TryGetComponent(out NavMeshAgent agent))
        {
            agent.ResetPath(); // 현재 경로 즉시 정지
            agent.velocity = Vector3.zero;
        }

        // 애니메이션 이동 상태 해제 (선택)
        if (TryGetComponent(out Animator anim))
        {
            anim.SetBool("isRunning", false);
            anim.SetBool("isRunning", false);
        }

        // 상태 플래그도 변경 (선택)
        canMove = false;
        isMove = false;
    }
    public void SpawnDashTrail(Vector3 start, Vector3 end)
    {
        if (skillHandler.skillData.trailEffectPrefab == null)
            return;

        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);

        //이펙트 위치
        Vector3 spawnPosition = start + direction * (distance / 2f);

        //회전
        Quaternion spawnRotation = Quaternion.LookRotation(direction);

        //이펙트 생성
        GameObject effect = GameObject.Instantiate(skillHandler.skillData.trailEffectPrefab, spawnPosition, spawnRotation);

        //이펙트 길이
        Vector3 effectScale = effect.transform.localScale;
        effect.transform.localScale = new Vector3(1.5f, 1.5f, distance);

        Destroy(effect, skillHandler.skillData.effectDuration);
    }
    public IEnumerator SpeedBoost(float duration, float boostedSpeed)
    {
        if (agent == null) yield break;

        float originalSpeed = agent.speed;
        agent.speed = boostedSpeed;

        yield return new WaitForSeconds(duration);

        agent.speed = originalSpeed;

        eSkillHandler?.EndSkillCast();
    }
    public void ResumeMovement()
    {
        canMove = true;
        skillHandler?.EndSkillCast();
    }
    public bool IsAttacking()
    {
        return isAttacking;
    }

    public void SkillEffect()
    {
        skillHandler.SpawnSkillEffect();
    }

    public void QskillSoundTiming()
    {
        skillHandler.QSkillSound();
    }
    public void OnRespawnButtonClicked()
    {
        BossController.isBossDead = false;
        SceneManager.LoadScene("BossRoom");
    }
    public void EnableWeaponCollider()
    {
        currentAttackHit = false;

        var weaponCollider = swordTransform.GetComponent<Collider>();
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }
    }
    public void DisableWeaponCollider()
    {
        var weaponCollider = swordTransform.GetComponent<Collider>();
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }

        int currentComboStep = comboStep;
        PlayAttackSound(currentComboStep);
    }
    public void PlayAttackSound(int step)
    {
        if (audioSource == null)
            return;

        if (currentAttackHit)
        {
            return;
        }

        AudioClip clipToPlay = null;

        if (step == 1 || step == 2)
        {
            clipToPlay = comboAttackSound;
        }
        else if (step == 3)
        {
            clipToPlay = LastComboSound;
        }

        if (clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay, attackSoundVolume);
        }
    }
    public void SetCurrentAttackHit(bool hit)
    {
        currentAttackHit = hit;
    }
    public void DisableSword()
    {
        swordObject.gameObject.SetActive(false);
    }
    
    public void EnableSword()
    {
        swordObject.gameObject.SetActive(true);
    }
}
