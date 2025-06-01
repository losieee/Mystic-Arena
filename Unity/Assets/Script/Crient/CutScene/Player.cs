using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor.TerrainTools;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.ComponentModel;

public class Player : MonoBehaviour
{
    public Animator animator;

    private bool canMove = true;
    private bool isMove = false;
    private bool isDashing = false;
    private bool isDead = false;

    private CapsuleCollider capsule;
    private NavMeshAgent agent;
    private Camera mainCamera;
    private Vector3 destination;
    private Coroutine comboResetCoroutine;

    private Quaternion idleAttackRotationOffset;    //애니메이션 기울이기용

    private void Awake()
    {
        capsule = GetComponentInChildren<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        mainCamera = Camera.main;

        // 오른쪽으로 30도 회전 오프셋 (Y축)
        idleAttackRotationOffset = Quaternion.Euler(0f, 30f, 0f);

        agent.updateRotation = false;
        agent.enabled = true;
        agent.acceleration = 1000f;
        agent.angularSpeed = 10000f;
        agent.stoppingDistance = 0.4f;
        agent.autoBraking = true;

    }
    private void Update()
    {
        if (canMove)
        {
            HandleKeyboardMoveInput();
        }

        LookMoveDirection();
    }
    private IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("BossRoom");
    }

    private void HandleKeyboardMoveInput()
    {
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
}
