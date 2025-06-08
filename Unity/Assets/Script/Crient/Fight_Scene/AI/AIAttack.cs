using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAttack : MonoBehaviour
{
    [Header("AI 설정")]
    public float detectionRadius = 10f; // 플레이어 감지 반경

    [Header("공격 설정")]
    public float attackRange = 2f;
    public float attackCooldown = 1f;

    [Header("HitBox 설정")]
    public GameObject leftHitBox;
    public GameObject bodyHitBox;

    [Header("체력 설정")]
    public float maxHealth = 100f;
    public float currentHealth;

    private float attackTimer = 0f;
    private bool isAttacking = false;
    private bool isPlayerDead = false;
    private bool isDead = false;

    private Vector3 fixedPosition;

    private Transform target; // 플레이어 위치 저장
    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        agent.updateRotation = true;
        agent.enabled = true;

        agent.speed = 8f;
        agent.acceleration = 1000f;
        agent.angularSpeed = 10000f;

        agent.stoppingDistance = attackRange - 0.3f;
        agent.autoBraking = true;

        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead)
        {
            agent.isStopped = true;
            return;
        }

        attackTimer += Time.deltaTime;
        isPlayerDead = Fight_Demo.isDead;

        if (isPlayerDead)
        {
            agent.isStopped = true;
            UpdateAnimation();
            return;
        }

        FindTarget();

        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);

            if (isAttacking)
            {
                agent.isStopped = true;
                UpdateAnimation();
                return;
            }

            if (distance <= attackRange)
            {
                agent.isStopped = true;

                if (attackTimer >= attackCooldown)
                {
                    attackTimer = 0f;
                    BasicAttack();
                }
            }
            else if (distance <= detectionRadius)
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);

                if (agent.velocity.sqrMagnitude > 0.01f)
                {
                    Vector3 lookDirection = agent.velocity.normalized;
                    lookDirection.y = 0f;

                    Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
                }
            }
            else
            {
                agent.isStopped = true;
            }
        }
        else
        {
            agent.isStopped = true;
        }

        UpdateAnimation();
    }

    void LateUpdate()
    {
        // 공격 중 위치 고정
        if (isAttacking)
        {
            Vector3 pos = fixedPosition;
            pos.y = transform.position.y; // y는 유지 (높이 변화는 허용)
            transform.position = pos;
        }
    }

    void FindTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        float closestDist = Mathf.Infinity;
        Transform closestTarget = null;

        foreach (GameObject obj in players)
        {
            float dist = Vector3.Distance(transform.position, obj.transform.position);

            if (dist < detectionRadius && dist < closestDist)
            {
                closestDist = dist;
                closestTarget = obj.transform;
            }
        }

        target = closestTarget;
    }

    void UpdateAnimation()
    {
        if (animator == null || agent == null) return;

        float speed = agent.velocity.magnitude;
        animator.SetFloat("MoveSpeed", speed);
    }

    void BasicAttack()
    {
        isAttacking = true;

        // 현재 위치 고정 기억
        fixedPosition = transform.position;

        agent.isStopped = true;

        // 공격 시작 전 → 바라보기
        if (target != null)
        {
            Vector3 lookDirection = (target.position - transform.position).normalized;
            lookDirection.y = 0f;

            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = targetRotation;
            }
        }

        animator.applyRootMotion = true; // Root Motion 유지
        animator.SetTrigger("Attack1");

        StartCoroutine(EndAttackAfterDelay(1f));
    }

    IEnumerator EndAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        isAttacking = false;
        animator.applyRootMotion = false; // 이동 시 다시 꺼줌

        // 공격 후 위치 동기화
        agent.Warp(transform.position);
        agent.isStopped = false;

        if (target != null && agent.isOnNavMesh)
        {
            agent.SetDestination(target.position);
        }
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Die();
            return;
        }

        // 강제 상태 초기화 (멈칫 처리)
        isAttacking = false;
        animator.applyRootMotion = false;
        agent.ResetPath();
        agent.isStopped = true;
        animator.SetFloat("MoveSpeed", 0f);
        animator.SetTrigger("Idle");

        // 멈칫 후 복귀 코루틴 실행
        StartCoroutine(RecoverFromHit(0.5f));
    }
    private IEnumerator RecoverFromHit(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!Fight_Demo.isDead && currentHealth > 0f)
        {
            agent.isStopped = false;
            if (target != null && agent.isOnNavMesh)
            {
                agent.SetDestination(target.position);
            }
        }
    }
    private void Die()
    {
        isDead = true;
        agent.isStopped = true;

        if (bodyHitBox != null)
            bodyHitBox.SetActive(false);

        animator.SetTrigger("isDead"); 
        Destroy(gameObject, 2f);
    }
    public void EndAttack()
    {
        isAttacking = false;
        animator.applyRootMotion = false;
    }
    public void EnableLeftHitBox()
    {
        if (leftHitBox != null)
            leftHitBox.SetActive(true);
    }

    public void DisableLeftHitBox()
    {
        if (leftHitBox != null)
            leftHitBox.SetActive(false);
    }
}
