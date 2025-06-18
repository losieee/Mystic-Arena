using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster4 : MonoBehaviour
{
    public EnemySO enemySO;
    public GameManager gameManager;

    //[Header("AI 설정")]
    //public float detectionRadius = 20f; // 플레이어 감지 반경

    [Header("공격 설정")]
    public float attackRange = 6f;
    public float attackCooldown = 3f;

    [Header("HitBox 설정")]
    public GameObject rock;
    public GameObject rockSpawn;
    public GameObject bodyHitBox;
    public AudioClip hitSound;

    private float attackTimer = 0f;
    private float animationLockTimer = 0f;
    private bool isAttacking = false;
    private bool isPlayerDead = false;
    private bool isDead = false;

    private Vector3 fixedPosition;

    private Transform target; // 플레이어 위치 저장
    private NavMeshAgent agent;
    private Animator animator;
    public float currentHp; // ? 개별 체력 변수
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        agent.updateRotation = true;
        agent.enabled = true;

        agent.speed = 7f;
        agent.acceleration = 1000f;
        agent.angularSpeed = 10000f;

        agent.stoppingDistance = attackRange - 0.3f;
        agent.autoBraking = true;

        currentHp = enemySO.monsterHp; // 개별 체력 초기화
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.IsDialoguePlaying())
        {
            agent.isStopped = true;
            animator.SetFloat("MoveSpeed", 0f);
            return;
        }

        if (isDead)
        {
            if (agent.enabled && agent.isOnNavMesh)
                agent.isStopped = true;
            animator.SetBool("IsRunning", false);
            return;
        }

        attackTimer += Time.deltaTime;
        isPlayerDead = Fight_Demo.isDead;

        if (isPlayerDead)
        {
            if (agent.enabled && agent.isOnNavMesh)
                agent.isStopped = true;
            animator.SetBool("IsRunning", false);
            return;
        }

        if (animationLockTimer > 0f)
            animationLockTimer -= Time.deltaTime;

        FindTarget();

        if (isAttacking)
        {
            if (agent.enabled && agent.isOnNavMesh)
                agent.isStopped = true;
            animator.SetBool("IsRunning", false);
            return;
        }

        if (animationLockTimer > 0f)
        {
            animationLockTimer -= Time.deltaTime;
        }

        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);

            if (distance <= attackRange)
            {
                if (agent.enabled && agent.isOnNavMesh)
                    agent.isStopped = true;

                if (attackTimer >= attackCooldown)
                {
                    attackTimer = 0f;
                    BasicAttack();
                }

                if (animationLockTimer <= 0f) UpdateAnimation();
                return;
            }
            else if (distance <= enemySO.monsterAttackInterval)
            {
                if (agent.enabled && agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                    agent.SetDestination(target.position);
                }
            }
            else
            {
                if (agent.enabled && agent.isOnNavMesh)
                    agent.isStopped = true;
            }
        }
        else
        {
            if (agent.enabled && agent.isOnNavMesh)
                agent.isStopped = true;
        }

        if (animationLockTimer <= 0f)
            UpdateAnimation();
    }

    void LateUpdate()
    {
        if (isAttacking)
        {
            transform.position = fixedPosition;
        }
        UpdateAnimation();
    }

    void FindTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        float closestDist = Mathf.Infinity;
        Transform closestTarget = null;

        foreach (GameObject obj in players)
        {
            float dist = Vector3.Distance(transform.position, obj.transform.position);

            if (dist < enemySO.monsterAttackInterval && dist < closestDist)
            {
                closestDist = dist;
                closestTarget = obj.transform;
            }
        }

        target = closestTarget;
    }

    void UpdateAnimation()
    {
        if (!agent.enabled || !agent.isOnNavMesh)
        {
            animator.SetBool("IsRunning", false);
            return;
        }

        bool isMoving = !agent.isStopped && agent.remainingDistance > agent.stoppingDistance;
        float speed = agent.velocity.magnitude;

        animator.SetBool("IsRunning", isMoving || speed > 0.05f);
    }

    void BasicAttack()
    {
        if (isAttacking) return;

        isAttacking = true;

        // 1. 가장 먼저 위치 고정
        fixedPosition = transform.position;

        // 2. 시선 고정
        if (target != null)
        {
            Vector3 lookDirection = (target.position - transform.position).normalized;
            lookDirection.y = 0f;
            if (lookDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }

        // 3. 애니메이션 설정
        animator.SetBool("isAttacking", true);
        animator.SetTrigger("Monster4_Attack");
        animator.applyRootMotion = true;

        // 4. NavMesh 중지
        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.isStopped = true;
            agent.updatePosition = false;
            agent.updateRotation = false;
        }
    }

    IEnumerator EndAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        isAttacking = false;

        if (animator != null)
        {
            // 기존 트리거 및 bool 모두 해제
            animator.applyRootMotion = false;
            animator.SetBool("isAttacking", false);
            animator.SetTrigger("Reset");
        }

        // NavMesh 복구
        if (!agent.enabled)
            agent.enabled = true;

        if (agent.isOnNavMesh)
        {
            transform.position = fixedPosition;
            agent.Warp(fixedPosition);

            agent.updatePosition = true;
            agent.updateRotation = true;
            agent.isStopped = false;

            if (target != null)
                agent.SetDestination(target.position);
        }

        UpdateAnimation();
    }
    public void TakeDamage(float damage)
    {
        currentHp -= damage;

        if (currentHp <= 0f)
        {
            if (hitSound != null)
            {
                AudioSource.PlayClipAtPoint(hitSound, transform.position, 1f);
            }
            Die();
            return;
        }

        // 강제 상태 초기화 (멈칫 처리)
        isAttacking = false;
        animator.applyRootMotion = false;
        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.isStopped = true;
        }
        animator.SetBool("IsRunning", false);
        animator.SetTrigger("Monster4_Hit");

        // 멈칫 후 복귀 코루틴 실행
        StartCoroutine(RecoverFromHit(0.5f));
    }
    private IEnumerator RecoverFromHit(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!Fight_Demo.isDead && currentHp > 0f)
        {
            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.updatePosition = true;
                agent.updateRotation = true;
                agent.isStopped = false;
                if (target != null)
                {
                    agent.SetDestination(target.position);
                }
            }

            UpdateAnimation();
        }
    }
    private void Die()
    {
        isDead = true;

        if (agent.enabled && agent.isOnNavMesh)
            agent.isStopped = true;

        if (bodyHitBox != null)
            bodyHitBox.SetActive(false);

        if (gameManager.aliveMonsterCount >= 0)
        {
            GameManager.instance.OnMonsterKilled();
        }

        animator.SetTrigger("isDead");
        Destroy(gameObject, 3f);
    }
    public void EndAttack()
    {
        isAttacking = false;

        if (animator != null)
        {
            animator.SetBool("isAttacking", false);
            animator.applyRootMotion = false;
        }

        if (!agent.enabled)
            agent.enabled = true;

        if (agent.isOnNavMesh)
        {
            agent.Warp(transform.position);
            agent.updatePosition = true;
            agent.updateRotation = true;
            agent.isStopped = false;

            if (target != null)
            {
                agent.SetDestination(target.position);
            }
        }

        UpdateAnimation();
    }
    public void EndHit()
    {
        if (isDead) return;

        animator.ResetTrigger("Monster4_Hit");

        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.updatePosition = true;
            agent.updateRotation = true;

            if (target != null)
                agent.SetDestination(target.position);
        }

        // 상태 복구
        isAttacking = false;
        animator.applyRootMotion = false;
        animator.SetBool("isAttacking", false);

        UpdateAnimation(); // Run 상태 복구
    }
    public void SpawnRock()
    {
        if (rock != null && rockSpawn != null && target != null)
        {
            GameObject newRock = Instantiate(rock, rockSpawn.transform.position, Quaternion.identity);

            Rigidbody rb = newRock.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = (target.position - rockSpawn.transform.position).normalized;

                direction.y = 0f;
                direction.Normalize();

                float throwForce = 20f;
                rb.AddForce(direction * throwForce, ForceMode.Impulse);
            }

            Destroy(newRock, 1f);
        }
    }
}
