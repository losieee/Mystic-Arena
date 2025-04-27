using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.PlasticSCM.Editor.WebApi;

public class Tutorial_Knight_Move : MonoBehaviour
{
    [Header("Stat Data")]
    [SerializeField] private Holly_Knight_Stats holly_Knight_Stats;

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
    public Transform spot;
    public Transform respawnPoint;
    public TutorialManager tutorialManager;

    private float curHealth;
    private float maxHealth;
    private float interactionRange;
    private float dashDistance;
    private float dashSpeed;
    private float respawnTime = 5f;
    private bool hasHealed = false;
    private bool isDead = false;
    private bool isDashing = false;
    private bool isInvincible = false;

    private NavMeshAgent agent;
    private CapsuleCollider capsule;
    private Light spotLight;
    private Camera mainCamera;
    private CanvasGroup healCanvasGroup;

    private Vector3 destination;
    private bool isMove = false;
    private bool canMove = false;

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
    }
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemySpawn") && !isDead)
        {
            // "EnemySpawn" 태그를 가진 콜라이더에 진입했고, 아직 죽지 않았다면 사망 처리
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
            int characterLayerMask = 1 << LayerMask.NameToLayer("Field");

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, characterLayerMask))
            {
                spot.position = hit.point;
                spot.transform.localScale = Vector3.one * 1.5f;
                spot.gameObject.SetActive(true);
                StartCoroutine(ShrinkSpot());
                SetDestination(hit.point);
            }
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("NPC") && !hasHealed &&
                    Vector3.Distance(transform.position, hit.point) <= interactionRange)
                {
                    if (tutorialManager != null && tutorialManager.IsWaitingForHeal())
                    {
                        tutorialManager.OnHealed(); // 다음 대사 이어짐
                    }
                    HealPlayer();
                    spot.gameObject.SetActive(false);
                    return;
                }
            }
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

    private IEnumerator ShrinkSpot()
    {
        float shrinkAmount = 0.3f;
        float duration = 0.1f;

        while (spot.transform.localScale.x > 0.1f)
        {
            spot.transform.localScale -= Vector3.one * shrinkAmount;
            yield return new WaitForSeconds(duration);
        }

        spot.gameObject.SetActive(false);
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
        if (!isMove) return;

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
        // 코루틴이 이미 실행 중이더라도 다시 시작하도록 수정 (리스폰 후 재사망 케이스)
        curHealth = 0;
        UpdateHealthUI();
        isDead = true;
        death.gameObject.SetActive(true);
        deathMark.gameObject.SetActive(true);
        agent.isStopped = true;
        agent.enabled = false;
        capsule.enabled = false;
        spotLight.enabled = false;
        spot.gameObject.SetActive(false);

        // 튜토리얼 매니저가 있고, 아직 힐 튜토리얼을 완료하지 않았다면 사망 처리
        if (tutorialManager != null && !hasHealed)
        {
            tutorialManager.OnCharacterDeath();
            yield break; // 리스폰 코루틴 종료
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
        isDead = false; // 리스폰 시 isDead를 false로 설정

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

    private IEnumerator DashForward()
    {
        isDashing = true;
        isInvincible = true;

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
}