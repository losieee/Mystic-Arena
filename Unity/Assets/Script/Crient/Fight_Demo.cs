using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Fight_Demo : MonoBehaviour
{
    [Header("Skill Handlers")]
    [SerializeField] private SkillHandler qSkillHandler;
    [SerializeField] private SkillHandler wSkillHandler;
    [SerializeField] private SkillHandler eSkillHandler;
    [SerializeField] private SkillHandler gSkillHandler;

    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashSpeed = 15f;

    public Animator animator;

    private bool canMove = true;
    private bool isMove = false;
    private bool isDashing = false;
    private bool isInvincible = false;

    // 콤보
    private int comboStep = 0;
    private bool isAttacking = false;
    private bool comboQueued = false;
    private bool canQueueNextCombo = false;

    private CapsuleCollider capsule;
    private NavMeshAgent agent;
    private Camera mainCamera;
    private Vector3 destination;

    private Quaternion idleAttackRotationOffset;

    private void Awake()
    {
        capsule = GetComponentInChildren<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        mainCamera = Camera.main;

        // 오른쪽으로 45도 회전 오프셋 (Y축)
        idleAttackRotationOffset = Quaternion.Euler(0f, 30f, 0f);

        // 극적인 방향회전 (일부러 한거임)
        agent.updateRotation = false;
        agent.enabled = true;
        agent.acceleration = 1000f;
        agent.angularSpeed = 10000f;
        agent.stoppingDistance = 0.4f;
        agent.autoBraking = true;

        gSkillHandler.onSkillUsed.AddListener(() => StartCoroutine(DashForward()));
    }

    private void Update()
    {
        HandleComboInput();

        if (canMove && !isAttacking)
        {
            HandleMoveInput();

            qSkillHandler.TryUseSkill();
            wSkillHandler.TryUseSkill();
            eSkillHandler.TryUseSkill();
            gSkillHandler.TryUseSkill();
        }

        LookMoveDirection();
    }

    private void HandleMoveInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                int fieldLayerMask = 1 << LayerMask.NameToLayer("Field");

                if (((1 << hit.collider.gameObject.layer) & fieldLayerMask) != 0)
                {
                    SetDestination(hit.point);
                }
            }
        }
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
            capsule.transform.rotation = Quaternion.LookRotation(direction);
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
        if (!Input.GetMouseButtonDown(0)) return;

        if (canQueueNextCombo && comboStep < 3)
        {
            comboQueued = true;
            return;
        }

        if (isAttacking) return;

        agent.ResetPath();
        isMove = false;

        // 회전 고정 (오른쪽 45도 오프셋 적용)
        capsule.transform.rotation = Quaternion.LookRotation(capsule.transform.forward) * idleAttackRotationOffset;

        comboStep = 1;
        isAttacking = true;
        animator.SetInteger("ComboCount", comboStep);
    }

    // 애니메이션 이벤트로 호출됨
    public void EnableNextCombo()
    {
        canQueueNextCombo = true;
    }

    // 애니메이션 종료 시 호출 (애니메이션 이벤트)
    public void EndCombo()
    {
        StartCoroutine(DelayedComboReset());
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
        }
    }

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
}