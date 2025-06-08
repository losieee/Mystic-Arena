using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AIAttack : MonoBehaviour
{
    [Header("AI ����")]
    public float detectionRadius = 50f;
    public float moveSpeed = 1f;
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    public int basicAttackToSkill = 4;

    [Header("ü�� ����")]
    public float maxHealth = 150f;
    public float currentHealth;

    [Header("���� ����")]
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private string attackStateName = "Attack";
    private WeaponDamage weaponDamageScript;

    [Header("UI ����")]
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

        // NavMeshAgent ���� (�ڿ������� �����ӿ�)
        agent.updateRotation = false;
        agent.acceleration = 1000f;
        agent.angularSpeed = 10000f;
        agent.stoppingDistance = 0.5f; // ���� �Ÿ��� �� ���缭 ����
        agent.autoBraking = true;

        // UI �ʱ�ȭ
        if (healthSliderUI != null)
        {
            healthSliderUI.SetTarget(transform);
            healthSliderUI.UpdateHealthBar(currentHealth, maxHealth);
        }
    }

    void Update()
    {
        FindClosestTarget();

        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            var playerMove = target.GetComponent<Fight_Demo>();

            // �÷��̾� ���� üũ
            if (playerMove != null)
            {
                target = null;
                StopAI();
                return;
            }

            // ȸ�� �ڿ������� (�÷��̾�ó��)
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0f; // �������θ� ȸ��
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }

            // ���� ���� üũ
            if (distance <= attackRange)
            {
                agent.isStopped = true; // ���� �� ����
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackCooldown)
                {
                    attackTimer = 0f;
                    BasicAttack();
                }
            }
            else
            {
                if (agent != null && agent.isOnNavMesh)
                {
                    agent.isStopped = false; // ���� ����� ������ �ٽ� Ǯ����
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

        // ���� �������� �̵� �簳 ���
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position); // ������ �ٽ� ����
        }
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
            Debug.Log("Die");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            TakeDamage(10f);
        }
    }
}
