using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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

    [Header("무기 설정")]
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private string attackStateName = "Attack";
    private WeaponDamage weaponDamageScript;

    [Header("UI 설정")]
    [SerializeField] private AISliderUI healthSliderUI;

    private float attackTimer = 0f;
    private int basicAttackCount = 0;
    private bool isDead = false;
    private bool isAttacking = false;

    private Transform target;
    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        weaponDamageScript = weaponCollider?.GetComponent<WeaponDamage>();

        currentHealth = maxHealth;

        if (weaponCollider != null)
            weaponCollider.enabled = false;

        // UI 초기화
        if (healthSliderUI != null)
        {
            healthSliderUI.SetTarget(transform); // AI 트랜스폼 설정
            healthSliderUI.UpdateHealthBar(currentHealth, maxHealth);
        }
    }

    void Update()
    {
        if (isDead) return;

        FindClosestTarget();

        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            var playerMove = target.GetComponentInParent<Tutorial_Knight_Move>();

            // 플레이어가 죽었거나 사망 상태이면 추적 중지
            if (playerMove != null && playerMove.isDead)
            {
                target = null;
                StopAI();
                return;
            }

            // 공격 가능 범위라면 공격
            if (distance <= attackRange)
            {
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
                // 추적 조건: 살아 있고 공격 중이 아닐 때만 이동
                if (!isAttacking && agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                    agent.SetDestination(target.position);
                }
            }
        }
        else
        {
            StopAI();
        }

        UpdateAnimation();
    }

    void LateUpdate()
    {
        if (animator == null || weaponCollider == null || weaponDamageScript == null) return;

        bool isInAttackAnimation = animator.GetCurrentAnimatorStateInfo(0).IsName(attackStateName);
        weaponCollider.enabled = isInAttackAnimation;

        if (isInAttackAnimation)
            weaponDamageScript.EnableDamage();
        else
            weaponDamageScript.DisableDamage();
    }

    void BasicAttack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
        StartCoroutine(EndAttackAfterDelay(1f));
    }

    IEnumerator EndAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isAttacking = false;
    }

    void UpdateAnimation()
    {
        if (animator == null || agent == null) return;
        bool isMoving = agent.velocity.magnitude > 0.1f;
        animator.SetBool("isRunning", isMoving);
    }

    void FindClosestTarget()
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

    void StopAI()
    {
        agent.ResetPath();
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        if (animator != null)
        {
            animator.SetBool("isRunning", false);
            animator.SetTrigger("Idle");
        }
    }

    public void TakeDamage(float dmg)
    {
        if (isDead) return;

        currentHealth -= dmg;

        
        if (healthSliderUI != null)
        {
            healthSliderUI.UpdateHealthBar(currentHealth, maxHealth);
        }

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

        FindObjectOfType<AIRespawn>()?.SpawnAI(5f);
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            TakeDamage(10f);
        }
    }
}
