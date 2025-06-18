using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster1 : MonoBehaviour
{
    public GameManager gameManager;
    public EnemySO enemySO; // 읽기 전용 템플릿

    //[Header("AI 설정")]
    //public float detectionRadius = 10f;

    [Header("공격 설정")]
    public float attackRange = 2f;
    public float attackCooldown = 1f;

    [Header("HitBox 설정")]
    public GameObject leftHitBox;
    public GameObject bodyHitBox;
    public AudioClip hitSound;

    private float attackTimer = 0f;
    private bool isAttacking = false;
    private bool isPlayerDead = false;
    private bool isDead = false;

    private Vector3 fixedPosition;

    private Transform target;
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

        agent.speed = 5f;
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
            else if (distance <= enemySO.monsterAttackInterval)
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
        if (animator == null || agent == null) return;

        float speed = agent.velocity.magnitude;
        animator.SetFloat("MoveSpeed", speed);
    }

    void BasicAttack()
    {
        isAttacking = true;
        fixedPosition = transform.position;
        agent.isStopped = true;

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

        animator.applyRootMotion = true;
        animator.SetTrigger("Attack1");
    }

    IEnumerator EndAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        isAttacking = false;
        animator.applyRootMotion = false;

        agent.Warp(transform.position);
        agent.isStopped = false;

        if (target != null && agent.isOnNavMesh)
        {
            agent.SetDestination(target.position);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage; // 개별 체력 차감

        if (currentHp <= 0f)
        {
            if (hitSound != null)
            {
                AudioSource.PlayClipAtPoint(hitSound, transform.position, 1f);
            }
            Die();
            return;
        }

        isAttacking = false;
        animator.applyRootMotion = false;
        agent.ResetPath();
        agent.isStopped = true;
        animator.SetFloat("MoveSpeed", 0f);
        animator.SetTrigger("Hit");

        StartCoroutine(RecoverFromHit(0.5f));
    }

    private IEnumerator RecoverFromHit(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!Fight_Demo.isDead && currentHp > 0f)
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

        if (gameManager.aliveMonsterCount >= 0)
        {
            GameManager.instance.OnMonsterKilled();
        }

        animator.SetTrigger("isDead");
        Destroy(gameObject, 2f);
    }

    public void EndAttack()
    {
        isAttacking = false;
        animator.applyRootMotion = false;

        agent.Warp(transform.position);
        agent.isStopped = false;

        if (target != null && agent.isOnNavMesh)
        {
            agent.SetDestination(target.position);
        }
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
