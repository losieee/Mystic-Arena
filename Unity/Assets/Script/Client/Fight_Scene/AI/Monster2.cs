using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster2 : MonoBehaviour
{
    public EnemySO enemySO;

    [Header("AI 설정")]
    public float detectionRadius = 10f; // 플레이어 감지 반경

    [Header("공격 설정")]
    public float attackRange = 2f;
    public float attackCooldown = 1f;

    [Header("HitBox 설정")]
    public GameObject rightHitBox;
    public GameObject bodyHitBox;
    public AudioClip hitSound;

    [Header("체력 설정")]
    public float maxHealth = 100f;
    public float currentHealth;

    private float attackTimer = 0f;
    private float animationLockTimer = 0f;
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

        agent.speed = 7f;
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
            animator.SetBool("IsRunning", false);
            return;
        }

        attackTimer += Time.deltaTime;
        isPlayerDead = Fight_Demo.isDead;

        if (isPlayerDead)
        {
            agent.isStopped = true;
            animator.SetBool("IsRunning", false);
            return;
        }

        if (animationLockTimer > 0f)
            animationLockTimer -= Time.deltaTime;

        FindTarget();

        if (isAttacking)
        {
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
                agent.isStopped = true;

                if (attackTimer >= attackCooldown)
                {
                    attackTimer = 0f;
                    BasicAttack();
                }

                if (animationLockTimer <= 0f) UpdateAnimation();
                return;
            }
            else if (distance <= detectionRadius)
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
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

        if (animationLockTimer <= 0f)
            UpdateAnimation();
    }

    void LateUpdate()
    {
        if (isAttacking)
        {
            Vector3 pos = fixedPosition;
            pos.y = transform.position.y;
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
        float speed = agent.velocity.magnitude;
        animator.SetBool("IsRunning", speed >= 0.05f);
    }

    void BasicAttack()
    {
        if (isAttacking) return;

        isAttacking = true;
        animator.SetBool("isAttacking", true);
        animator.SetTrigger("Monster2_Attack");

        agent.isStopped = true;
        animator.applyRootMotion = true;
        fixedPosition = transform.position;

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

        StartCoroutine(EndAttackAfterDelay(1.8f));
    }

    IEnumerator EndAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        isAttacking = false;
        animator.SetBool("isAttacking", false);
        animator.applyRootMotion = false;

        agent.Warp(transform.position);
        agent.isStopped = false;
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position, 1f);
        }


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
        animator.SetBool("IsRunning", false);
        animator.SetTrigger("Monster2_Hit");

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
        Destroy(gameObject, 3f);
    }
    public void EndAttack()
    {
        isAttacking = false;
        animator.applyRootMotion = false;

        // 위치 보정
        agent.Warp(transform.position);
        agent.isStopped = false;

        if (target != null && agent.isOnNavMesh)
        {
            agent.SetDestination(target.position);
        }
    }
    public void EnableLeftHitBox()
    {
        if (rightHitBox != null)
            rightHitBox.SetActive(true);
    }

    public void DisableLeftHitBox()
    {
        if (rightHitBox != null)
            rightHitBox.SetActive(false);
    }
}
