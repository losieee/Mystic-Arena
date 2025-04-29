using Fusion.Encryption;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAttack : MonoBehaviour
{
    [Header("AI 설정")]
    public float detectionRadius = 50f;
    public float moveSpeed = 1f;
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    public int basicAttackToSkill = 4;

    [Header("체력 설정")]
    public float maxHealth = 150f;
    public float currentHealth;

    private float attackTimer = 0f;
    private int basicAttackCount = 0;
    private bool isDead = false;

    private Transform target;
    private NavMeshAgent agent;
    private Animator animator;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead) return;

        FindClosestTarget();

        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);

            if (distance <= attackRange)
            {
                // 제자리에서 공격
                agent.ResetPath();
                transform.LookAt(target);

                attackTimer += Time.deltaTime;
                if (attackTimer >= attackCooldown)
                {
                    attackTimer = 0f;
                    BasicAttack();
                }
            }
            else
            {
                agent.SetDestination(target.position);
            }
            // 플레이어가 죽었는지 확인하고 AI 행동 변경
            Tutorial_Knight_Move playerMove = target.GetComponentInParent<Tutorial_Knight_Move>();
            if (playerMove != null && playerMove.isDead)
            {
                // 타겟이 죽었으므로 추적 및 공격 중단
                target = null;
                agent.ResetPath();
                agent.isStopped = true;
                agent.velocity = Vector3.zero;

                if (animator != null)
                {
                    animator.SetBool("isRunning", false);
                    animator.SetTrigger("Idle");
                }
            }
            else if (playerMove != null && !playerMove.isDead && distance <= attackRange)
            {
                // 공격 범위 내에 살아있는 플레이어가 있다면 공격 상태 유지
                agent.isStopped = false;
            }
            else if (target != null)
            {
                // 타겟이 살아있고 공격 범위 밖이라면 추격
                agent.isStopped = false;
            }
        }
        else
        {
            // 타겟이 없으면 이동 중지
            agent.ResetPath();
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            if (animator != null)
            {
                animator.SetBool("isRunning", false);
                animator.SetTrigger("Idle");
            }
        }

        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        if (animator == null || agent == null) return;

        bool isMoving = agent.velocity.magnitude > 0.1f;
        animator.SetBool("isRunning", isMoving);
    }

    void FindClosestTarget()
    {
        GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag("Player");
        List<GameObject> allTargets = new List<GameObject>(potentialTargets);

        GameObject[] aiTargets = GameObject.FindGameObjectsWithTag("AI");
        foreach (GameObject ai in aiTargets)
        {
            if (ai != this.gameObject)
                allTargets.Add(ai);
        }

        float closestDist = Mathf.Infinity;
        Transform closestTarget = null;

        foreach (GameObject obj in allTargets)
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    void BasicAttack()
    {
        animator.SetTrigger("Attack");
        DealDamageToPlayer();
        basicAttackCount++;

        if (basicAttackCount >= basicAttackToSkill)
        {
            UseSkill();
            basicAttackCount = 0;
        }
    }

    void UseSkill()
    {
        animator.SetTrigger("Qskill");
    }

    public void TakeDamage(float dmg)
    {
        if (isDead) return;

        currentHealth -= dmg;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        agent.ResetPath();
        animator.SetTrigger("isDead");
        Destroy(gameObject, 3f);
    }

    // 무기 콜라이더에 닿았을 때 데미지 받기
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            TakeDamage(10f);
        }
    }
    // AI공격
    public void DealDamageToPlayer()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= attackRange && target.CompareTag("Player"))
        {
            Tutorial_Knight_Move playerMove = target.GetComponentInParent<Tutorial_Knight_Move>();
            if (playerMove != null)
            {
                Debug.Log("AI의 공격");
                playerMove.Damage(10f);
            }
            if (playerMove.isDead)
            {

            }
        }
    }
}
