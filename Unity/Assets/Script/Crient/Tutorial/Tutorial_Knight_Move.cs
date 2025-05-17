using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
//using Unity.PlasticSCM.Editor.WebApi;
//using Unity.VisualScripting;

public class Tutorial_Knight_Move : MonoBehaviour
{
    [Header("Stat Data")]
    [SerializeField] private PlayerStats holly_Knight_Stats;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;

    [Header("Skill Handlers")]
    [SerializeField] private SkillHandler qSkillHandler;
    [SerializeField] private SkillHandler wSkillHandler;
    [SerializeField] private SkillHandler eSkillHandler;
    [SerializeField] private SkillHandler gSkillHandler;

    [Header("Health UI")]
    [SerializeField] private Image infoBarImage;
    [SerializeField] private Image characterBarImage;
    [SerializeField] private Image death;
    [SerializeField] private Image lowHp;
    [SerializeField] private Image heal;
    [SerializeField] private Transform deathMark;
    [SerializeField] private ParticleSystem healEffect;
    [SerializeField] private TextMeshProUGUI respawnCountdownText;

    [Header("Field of View")]
    [SerializeField] private FieldOfView fieldOfView;
    [SerializeField] private SmallFieldOfView smallFieldOfView;

    [Header("Other")]
    public Transform respawnPoint;
    public TutorialManager tutorialManager;
    public Animator animator;

    private float curHealth;
    private float maxHealth;
    private float interactionRange;
    private float dashDistance;
    private float dashSpeed;
    private float respawnTime = 5f;
    private float attackTimer = 0f;

    private bool hasHealed = false;
    public bool isDead = false;
    private bool isDashing = false;
    private bool isInvincible = false;
    private bool isMove = false;
    private bool canMove = false;
    private bool hasDied = false;

    private NavMeshAgent agent;
    private CapsuleCollider capsule;
    private Light spotLight;
    private Camera mainCamera;
    private CanvasGroup healCanvasGroup;
    private Vector3 destination;
    private List<Renderer> currentObstructions = new List<Renderer>();
    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    private Transform attackTarget = null;


    private void Awake()
    {
        Debug.Log(isDashing);
        curHealth = holly_Knight_Stats.maxHealth;
        maxHealth = holly_Knight_Stats.maxHealth;
        dashDistance = holly_Knight_Stats.dashDistance;
        dashSpeed = holly_Knight_Stats.dashSpeed;
        interactionRange = holly_Knight_Stats.interactionRange;
        gSkillHandler.onSkillUsed.AddListener(() => StartCoroutine(DashForward()));

        mainCamera = Camera.main;
        capsule = GetComponentInChildren<CapsuleCollider>();
        spotLight = GetComponentInChildren<Light>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        agent.updateRotation = false;
        agent.enabled = true;

        death.gameObject.SetActive(false);
        lowHp.gameObject.SetActive(false);
        heal.gameObject.SetActive(false);
        deathMark.gameObject.SetActive(false);
    }

    private void Start()
    {
        mainCamera = Camera.main;
        healCanvasGroup = heal.GetComponent<CanvasGroup>() ?? heal.gameObject.AddComponent<CanvasGroup>();
        healCanvasGroup.alpha = 0f;
        animator.applyRootMotion = false;
        // 애니메이션 초기화(시작하자마자 스킬 쓰는것 방지)
        animator.ResetTrigger("Qskill");
    }
    // 캐릭터 이동
    public void EnableMovement()
    {
        canMove = true;
    }

    private void Update()
    {
        if (canMove)
        {
            if (!isDead)
            {
                HandleMoveInput();
                qSkillHandler.TryUseSkill();
                wSkillHandler.TryUseSkill();
                eSkillHandler.TryUseSkill();
                gSkillHandler.TryUseSkill();
            }
        }

        LookMoveDirection();

        if (isDead && lowHp.gameObject.activeSelf)
            lowHp.gameObject.SetActive(false);

        HandleCameraObstructions();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemySpawn") && !isDead)
        {
            StartCoroutine(RespawnCoroutine());
        }
    }

    private void HandleMoveInput()
    {
        if (tutorialManager != null && tutorialManager.IsInputBlocked())
            return;

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 적 감지
                if (hit.collider.CompareTag("AI"))
                {
                    attackTarget = hit.collider.transform;
                    SetDestination(attackTarget.position);
                    return;
                }

                // NPC 힐 처리
                if (hit.collider.CompareTag("NPC") && !hasHealed &&
                    Vector3.Distance(transform.position, hit.point) <= interactionRange)
                {
                    if (tutorialManager != null && tutorialManager.IsWaitingForHeal())
                        tutorialManager.OnHealed();

                    HealPlayer();
                    return;
                }
                // 이미 힐 했을때 다시 힐 하면 텍스트 활성화
                else if (hit.collider.CompareTag("NPC") && hasHealed &&
                        Vector3.Distance(transform.position, hit.point) <= interactionRange)
                {
                    var healNotice = hit.collider.GetComponentInChildren<TextMeshProUGUI>(true);
                    if (healNotice != null)
                    {
                        healNotice.gameObject.SetActive(true);
                        StartCoroutine(HideAfterSeconds(healNotice.gameObject, 2f));
                    }
                }

                // 일반 이동 처리
                int characterLayerMask = 1 << LayerMask.NameToLayer("Field");
                if (((1 << hit.collider.gameObject.layer) & characterLayerMask) != 0)
                {
                    attackTarget = null;
                    SetDestination(hit.point);
                }
            }
        }
        if (attackTarget != null && !isDead)
        {
            float dist = Vector3.Distance(transform.position, attackTarget.position);
            if (dist <= attackRange)
            {
                agent.ResetPath();
                transform.LookAt(attackTarget);

                attackTimer += Time.deltaTime;
                if (attackTimer >= attackCooldown)
                {
                    attackTimer = 0f;
                    PerformBasicAttack();
                }
            }
        }
    }
    // 힐 텍스트 자동 비활성화
    private IEnumerator HideAfterSeconds(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }
    // 기본공격
    private void PerformBasicAttack()
    {
        animator.SetTrigger("Attack");

        // 데미지 처리
        var enemy = attackTarget.GetComponent<AIAttack>();
        if (enemy != null)
        {
            enemy.TakeDamage(10f);
        }
    }

    private void HealPlayer()
    {
        if (curHealth >= maxHealth) return;

        curHealth += holly_Knight_Stats.HealAmount;
        curHealth = Mathf.Min(curHealth, maxHealth);

        StartCoroutine(ShowHealEffectSmooth());
        UpdateHealthUI();
        hasHealed = true;

        healEffect?.Play();
    }

    private IEnumerator ShowHealEffectSmooth()
    {
        heal.gameObject.SetActive(true);

        for (float t = 0; t < 1f; t += Time.deltaTime / 0.3f)
        {
            healCanvasGroup.alpha = t;
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        for (float t = 1f; t > 0f; t -= Time.deltaTime / 0.3f)
        {
            healCanvasGroup.alpha = t;
            yield return null;
        }

        healCanvasGroup.alpha = 0f;
        heal.gameObject.SetActive(false);
    }

    private void SetDestination(Vector3 dest)
    {
        agent.SetDestination(dest);
        destination = dest;
        isMove = true;

        Vector3 direction = (destination - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            direction.y = 0;
            capsule.transform.forward = direction;
        }
    }

    private void LookMoveDirection()
    {
        if (!isMove)
        {
            animator.SetBool("isRunning", false);
            return;
        }

        Vector3 moveDirection = agent.velocity.normalized;
        if (moveDirection != Vector3.zero)
        {
            moveDirection.y = 0;
            capsule.transform.forward = moveDirection;
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

    public void Damage(float damage)
    {
        if (isDead || isInvincible || curHealth <= 0) return;

        curHealth -= damage;
        if (curHealth <= 0)
            StartCoroutine(RespawnCoroutine());

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        float ratio = curHealth / maxHealth;
        infoBarImage.fillAmount = ratio;
        characterBarImage.fillAmount = ratio;

        if (ratio <= 0.1f)
        {
            infoBarImage.color = Color.red;
            lowHp.gameObject.SetActive(true);
        }
        else if (ratio <= 0.5f)
        {
            infoBarImage.color = new Color(1f, 0.5f, 0f);
            lowHp.gameObject.SetActive(true);
        }
        else
        {
            infoBarImage.color = Color.white;
            lowHp.gameObject.SetActive(false);
        }
    }

    private IEnumerator RespawnCoroutine()
    {
        // 죽으면 체력은 0으로 고정
        curHealth = 0;
        UpdateHealthUI();
        isDead = true;
        // 사망 애니메이션 한번만 실행
        if (isDead && !hasDied)
        {
            animator.SetTrigger("isDead");
            hasDied = true;
        }

        death.gameObject.SetActive(true);
        deathMark.gameObject.SetActive(true);
        agent.isStopped = true;
        agent.enabled = false;
        capsule.enabled = false;
        spotLight.enabled = false;

        // 튜토리얼 매니저가 있고, 아직 힐 튜토리얼을 완료하지 않았다면 사망 처리
        if (tutorialManager != null && !hasHealed)
        {
            tutorialManager.OnCharacterDeath();
        }

        float countdown = respawnTime;
        while (countdown > 0)
        {
            if (respawnCountdownText != null)
                respawnCountdownText.text = Mathf.Ceil(countdown).ToString();

            yield return new WaitForSeconds(1f);
            countdown--;
        }

        Respawn();
    }

    private void Respawn()
    {
        transform.SetPositionAndRotation(respawnPoint.position, respawnPoint.rotation);
        death.gameObject.SetActive(false);
        deathMark.gameObject.SetActive(false);
        capsule.enabled = true;
        agent.enabled = true;
        agent.isStopped = false;
        spotLight.enabled = true;

        curHealth = maxHealth;
        hasHealed = false;
        isDead = false;
        hasDied = false;

        // 시야 정보 초기화
        if (fieldOfView != null)
        {
            fieldOfView.HideAllVisibleTargets();
        }

        UpdateHealthUI();
        healEffect?.Play();

        agent.ResetPath();
        isMove = false;

        // 리스폰 시 튜토리얼 매니저에 알림
        if (tutorialManager != null)
        {
            tutorialManager.OnCharacterRespawn();
        }
    }
    // 대쉬
    private IEnumerator DashForward()
    {
        isDashing = true;
        isInvincible = true;
        animator.SetTrigger("isDashing");

        float dashTime = dashDistance / dashSpeed;
        Vector3 dashDirection = capsule.transform.forward;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, dashDirection, out hit, dashDistance))
        {
            dashDistance = hit.distance - 0.3f;
        }

        float elapsed = 0f;
        Vector3 start = transform.position;
        Vector3 end = start + dashDirection * dashDistance;

        while (elapsed < dashTime)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / dashTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        isDashing = false;
        isInvincible = false;
    }

    public void SetHealthToLow()
    {
        curHealth = 10f;
        UpdateHealthUI();
    }
    public void StopAgentImmediately()
    {
        if (agent != null)
        {
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }
    }
    // 스킬 쓰는 도중 움직일수 없게
    public IEnumerator DisableMovementDuringSkill(float duration)
    {
        canMove = false;
        yield return new WaitForSeconds(duration);
        canMove = true;
    }

    // 카메라와 캐릭터 사이 장애물 처리
    private void HandleCameraObstructions()
    {
        foreach (var rend in currentObstructions)
        {
            if (rend != null && originalMaterials.ContainsKey(rend))
            {
                rend.materials = originalMaterials[rend];
            }
        }

        currentObstructions.Clear();
        originalMaterials.Clear();

        Vector3 direction = transform.position - mainCamera.transform.position;
        float distance = Vector3.Distance(mainCamera.transform.position, transform.position);

        RaycastHit[] hits = Physics.RaycastAll(mainCamera.transform.position, direction, distance);

        foreach (RaycastHit hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null && rend.gameObject != gameObject && !currentObstructions.Contains(rend))
            {
                originalMaterials[rend] = rend.materials;

                Material[] transparentMats = new Material[rend.materials.Length];
                for (int i = 0; i < transparentMats.Length; i++)
                {
                    Material mat = new Material(rend.materials[i]);
                    mat.SetFloat("_Mode", 2); // Fade 모드
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.EnableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = 3000;
                    Color c = mat.color;
                    c.a = 0.3f;
                    mat.color = c;
                    transparentMats[i] = mat;
                }

                rend.materials = transparentMats;
                currentObstructions.Add(rend);
            }
        }
    }
    public IEnumerator DisableRootMotionAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        animator.applyRootMotion = false;
    }
}