using UnityEngine;
using UnityEngine.AI;
using System.Collections;
//using System.Text.RegularExpressions;
//using UnityEditor.TerrainTools;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static System.Net.Mime.MediaTypeNames;
using TMPro;
//using System.ComponentModel;
//using Unity.Properties;

public class Fight_Demo : MonoBehaviour
{
    public WeaponSO weaponSO;
    public PlayerSO playerSO;

    public float player_currHp;

    [Header("Skill Handlers")]
    [SerializeField] private SkillHandler qSkillHandler;
    [SerializeField] private SkillHandler eSkillHandler;
    [SerializeField] private SkillHandler shiftSkillHandler;

    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashSpeed = 15f;

    [Header("Weapon Text")]
    public GameObject textPanel;
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;
    public TextMeshProUGUI text3;
    public TextMeshProUGUI text4;
    public TextMeshProUGUI text5;
    public GameObject qskillImage;
    public GameObject shiftSkillImage;
    public GameObject eSkillImage;

    public Animator animator;
    public Transform swordTransform;
    public Transform swordObject;
    public AudioClip comboAttackSound;
    public AudioClip LastComboSound;
    public AudioClip clickSound;
    public AudioClip attackSound;
    public AudioClip deathSound;

    public SkillHandler skillHandler;
    public UnityEngine.UI.Image hpBarImage;
    public GameObject deathPanel;

    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool isWorking = false;
    private bool isMove = false;
    private bool isDashing = false;
    private bool isInvincible = false;
    public static bool isDead = false;
    private bool currentAttackHit = false;
    private bool isHit = false;
    private bool queuedAttackInput = false;
    private bool isDialoguePlaying = false;
    private bool inputLocked = false;

    // �޺�
    private int comboStep = 0;
    private bool isAttacking = false;
    private bool comboQueued = false;
    private bool canQueueNextCombo = false;
    private float attackSoundVolume = 0.2f;

    private CapsuleCollider capsule;
    public NavMeshAgent agent;
    private Camera mainCamera;
    private Vector3 destination;
    private Coroutine comboResetCoroutine;
    private AudioSource audioSource;
    private SceneSequenceManager sceneManager;


    private Quaternion idleAttackRotationOffset;    //�ִϸ��̼� ����̱��
    private Coroutine typingCoroutine = null;

    private void Awake()
    {
        capsule = GetComponentInChildren<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        mainCamera = Camera.main;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Y������ 30�� ȸ�� ������
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
        player_currHp = playerSO.playerMaxHp;
        UpdateHPUI();
        Debug.Log(isDashing);

        isDead = false;
        isHit = false;
        canMove = true;
        isMove = false;
        isDashing = false;
        isInvincible = false;
        player_currHp = playerSO.playerMaxHp;

        animator.SetBool("isDead", false);

        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible || isHit)
            return;

        player_currHp -= damage;
        player_currHp = Mathf.Clamp(player_currHp, 0, playerSO.playerMaxHp);
        UpdateHPUI();

        if (player_currHp <= 0 && !isDead)
        {
            Dead();
            return;
        }

        StartCoroutine(PlayHitAnimation());
    }
    public void Dead()
    {
        isDead = true;
        animator.SetBool("isDead", true);
        agent.ResetPath();
        animator.SetBool("isRunning", false);
        animator.SetBool("isAttacking", false);

        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound, 0.5f);
        }

        if (deathPanel != null)
            deathPanel.SetActive(true);
    }
    private IEnumerator PlayHitAnimation()
    {
        isHit = true;
        canMove = false;
        isAttacking = false;

        // ���� ���
        comboStep = 0;
        comboQueued = false;
        canQueueNextCombo = false;
        animator.SetInteger("ComboCount", 0);
        animator.SetBool("isAttacking", false);

        animator.SetTrigger("Hit");

        // Hit �ִϸ��̼� ���̸�ŭ ���
        yield return new WaitForSeconds(0.16f);

        isHit = false;
        canMove = true;

        // �ǰ� ���� �� ���� �Է� �־����� ����
        if (queuedAttackInput && !isDead)
        {
            queuedAttackInput = false;
            StartAttackCombo(); // �Լ��� �и��ؼ� ���
        }
    }
    public void UpdateHPUI()
    {
        if (hpBarImage != null)
        {
            hpBarImage.fillAmount = player_currHp / playerSO.playerMaxHp;
        }
    }
    private void Update()
    {
        EnsureMainCamera();

        if (inputLocked) return;

        if (isWorking || isDead || isDialoguePlaying) return;

        // ���� ��� ��
        if (BossController.isBossDead)
        {
            agent.ResetPath();
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", false);
            return;
        }

        // �÷��̾� ��� ��
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

        // ��� ���� ó��
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

            if (Input.GetKeyDown(KeyCode.LeftShift))
                shiftSkillHandler.TryUseSkill();
        }

        LookMoveDirection();
    }
    private void EnsureMainCamera()
    {
        if (mainCamera == null || !mainCamera.enabled)
        {
            mainCamera = Camera.main;
        }
    }

    private void HandleKeyboardMoveInput()
    {
        if (!canMove) return;

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

        if (Input.GetMouseButtonDown(0))
        {
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

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, 100f);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            Vector3? targetPoint = null;
            foreach (var hit in hits)
            {
                if (ObstructionManager.transparentObjects.Contains(hit.collider.gameObject))
                    continue;

                targetPoint = hit.point;
                break;
            }

            if (targetPoint.HasValue)
            {
                Vector3 dir = targetPoint.Value - transform.position;
                dir.y = 0;

                if (dir.sqrMagnitude > 0.01f)
                {
                    capsule.transform.rotation = Quaternion.LookRotation(dir) * idleAttackRotationOffset;
                }
            }

            StartAttackCombo();
        }
    }

    private void StartAttackCombo()
    {
        DontMove();

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

            canMove = true;
            animator.SetBool("isAttacking", false);
        }
    }

    public IEnumerator DashForward()
    {
        if (isAttacking)
        {
            // ��� �߿��� ���� ���¸� ����
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

        // ��� �Ŀ��� ������ ����
        canMove = true;
    }
    public void StopAgentImmediately()
    {
        if (TryGetComponent(out NavMeshAgent agent))
        {
            agent.ResetPath(); // ���� ��� ��� ����
            agent.velocity = Vector3.zero;
        }

        // �ִϸ��̼� �̵� ���� ���� (����)
        if (TryGetComponent(out Animator anim))
        {
            anim.SetBool("isRunning", false);
            anim.SetBool("isRunning", false);
        }

        // ���� �÷��׵� ���� (����)
        canMove = false;
        isMove = false;
    }
    public void SpawnDashTrail(Vector3 start, Vector3 end)
    {
        if (skillHandler.skillData.trailEffectPrefab == null)
            return;

        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);

        //����Ʈ ��ġ
        Vector3 spawnPosition = start + direction * (distance / 2f);

        //ȸ��
        Quaternion spawnRotation = Quaternion.LookRotation(direction);

        //����Ʈ ����
        GameObject effect = GameObject.Instantiate(skillHandler.skillData.trailEffectPrefab, spawnPosition, spawnRotation);

        //����Ʈ ����
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

        if (deathPanel != null)
            deathPanel.SetActive(false);

        if (TryGetComponent(out Fight_Demo player))
        {
            player.playerSO.playerCurrHp = player.playerSO.playerMaxHp;
            player.player_currHp = player.playerSO.playerMaxHp;
            player.UpdateHPUI();

            isDead = false;
            player.canMove = true;
            player.animator.SetBool("isDead", false);
        }

        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }
    public void EnableWeaponCollider()
    {
        EnableActiveWeaponCollider(swordObject);
    }
    public void DisableWeaponCollider()
    {
        DisableActiveWeaponCollider(swordObject);
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
    public void DisableActiveWeaponCollider(Transform weaponRoot)
    {
        foreach (Transform child in weaponRoot)
        {
            if (!child.gameObject.activeSelf)
                continue;

            // ���� Ȱ��ȭ�� ������ ���� Ʈ������ �߿��� BoxCollider ã��
            BoxCollider box = child.GetComponentInChildren<BoxCollider>(true);
            if (box != null)
            {
                box.enabled = false;
            }
        }
    }

    public void EnableActiveWeaponCollider(Transform weaponRoot)
    {
        foreach (Transform child in weaponRoot)
        {
            if (!child.gameObject.activeSelf)
                continue;

            // ���� Ȱ��ȭ�� ������ ���� Ʈ������ �߿��� BoxCollider ã��
            BoxCollider box = child.GetComponentInChildren<BoxCollider>(true);
            if (box != null)
            {
                box.enabled = true;
            }
        }
    }
    public void RevivePlayer()
    {
        isDead = false;
        isHit = false;
        isDashing = false;
        canMove = true;
        isMove = false;

        player_currHp = playerSO.playerMaxHp;
        playerSO.playerCurrHp = playerSO.playerMaxHp;
        UpdateHPUI();

        animator.SetBool("isDead", false);

        if (deathPanel != null)
            deathPanel.SetActive(false);
    }
    public void PlayClickSound()
    {
        if (clickSound != null)
        {
            audioSource.clip = clickSound;
            audioSource.volume = 0.1f;
            audioSource.Play();
        }
    }

    public void WeaponText()
    {

        string[] dialogueLines = new string[]
        {
            "���ù��� ��\n\n�̰� ����?",
            "���� ���� ��ű�\n\nȮ�� ���, ������ ���� �ӿ��� ������ �����Դϴ�. �ǹ��δ�... ó�� ���׿�",
            "���� ���� ��ű�\n\n�� ������ �̸��� �ð���, �� �ź��� ���� ������ ��Ģ���� ������� �� �ִٰ� �������ϴ�.",
            "���� ���� ��ű�\n\n����� �ƴϸ� �ٷ� �� �����̴ϴ�.",
            "���ù��� ��\n\n������ ���� �ǳ� �ˡ� �̰� �ܼ��� ���Ⱑ �ƴϾ�. �� ��, ������ ������ ���ư��� ��."
        };

        qskillImage?.SetActive(false);
        shiftSkillImage?.SetActive(false);
        eSkillImage?.SetActive(false);

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(ShowDialogueLineByLine(dialogueLines, 0.05f, 1.0f));
    }
    private IEnumerator ShowDialogueLineByLine(string[] lines, float charDelay, float lineDelay)
    {
        isDialoguePlaying = true;
        canMove = false;
        isAttacking = false;
        comboStep = 0;

        if (textPanel != null)
            textPanel.SetActive(true);

        if (text1 != null) text1.text = "";

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            text1.text = ""; // ���� �� ����

            for (int j = 0; j < line.Length; j++)
            {
                text1.text += line[j];
                yield return new WaitForSeconds(charDelay);
            }

            yield return new WaitForSeconds(lineDelay);
        }

        yield return new WaitForSeconds(2f);
        if (textPanel != null)
            textPanel.SetActive(false);

        isDialoguePlaying = false;
        canMove = true;

        qskillImage?.SetActive(true);
        shiftSkillImage?.SetActive(true);
        eSkillImage?.SetActive(true);
    }
    public void SetInputLock(bool locked)
    {
        inputLocked = locked;
    }
}
