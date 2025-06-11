using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster2 : MonoBehaviour
{
    public EnemySO enemySO;

    [Header("AI ����")]
    public float detectionRadius = 10f; // �÷��̾� ���� �ݰ�

    [Header("���� ����")]
    public float attackRange = 2f;
    public float attackCooldown = 1f;

    [Header("HitBox ����")]
    public GameObject rightHitBox;
    public GameObject bodyHitBox;
    public AudioClip hitSound;

    [Header("ü�� ����")]
    public float maxHealth = 100f;
    public float currentHealth;

    private float attackTimer = 0f;
    private float animationLockTimer = 0f;
    private bool isAttacking = false;
    private bool isPlayerDead = false;
    private bool isDead = false;

    private Vector3 fixedPosition;

    private Transform target; // �÷��̾� ��ġ ����
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
            else if (distance <= detectionRadius)
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

        // 1. ���� ���� ��ġ ����
        fixedPosition = transform.position;

        // 2. �ü� ����
        if (target != null)
        {
            Vector3 lookDirection = (target.position - transform.position).normalized;
            lookDirection.y = 0f;
            if (lookDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }

        // 3. �ִϸ��̼� ����
        animator.SetBool("isAttacking", true);
        animator.SetTrigger("Monster2_Attack");
        animator.applyRootMotion = true;

        // 4. NavMesh ����
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
            // ���� Ʈ���� �� bool ��� ����
            animator.applyRootMotion = false;
            animator.SetBool("isAttacking", false);  
            animator.SetTrigger("Reset");           
        }

        // NavMesh ����
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

        // ���� ���� �ʱ�ȭ (��ĩ ó��)
        isAttacking = false;
        animator.applyRootMotion = false;
        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.isStopped = true;
        }
        animator.SetBool("IsRunning", false);
        animator.SetTrigger("Monster2_Hit");

        // ��ĩ �� ���� �ڷ�ƾ ����
        StartCoroutine(RecoverFromHit(0.5f));
    }
    private IEnumerator RecoverFromHit(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!Fight_Demo.isDead && currentHealth > 0f)
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
    public void EndHit()
    {
        if (isDead) return;

        animator.ResetTrigger("Monster2_Hit");

        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.updatePosition = true;
            agent.updateRotation = true;

            if (target != null)
                agent.SetDestination(target.position);
        }

        // ���� ����
        isAttacking = false;
        animator.applyRootMotion = false;
        animator.SetBool("isAttacking", false);

        UpdateAnimation(); // Run ���� ����
    }
}
