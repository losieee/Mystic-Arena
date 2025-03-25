using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    // 플레이어 이동 속도
    public float speed;

    // 플레이어 체력
    float curHealth;    // 현재 체력
    public float maxHealth;     // 최대 체력
    
    private Camera camera;
    private CapsuleCollider capsule;
    private NavMeshAgent agent;

    public Transform spot;

    private bool isMove = false;

    private Vector3 destination;


    private void Awake()
    {
        camera = Camera.main;
        capsule = GetComponentInChildren<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;   // NavMeshAgent의 회전을 비활성화
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit))
            {
                spot.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                spot.position = hit.point;
                spot.gameObject.SetActive(true);
                SetDestination(hit.point);
            }
        }
        LookMoveDirection();
    }

    private void SetDestination(Vector3 dest)
    {
        agent.SetDestination(dest);
        destination = dest;
        isMove = true;

        Vector3 direction = (destination - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            direction.y = 0; // y 값을 0으로 고정하여 수평 방향으로만 회전하도록 설정
            capsule.transform.forward = direction;
        }
    }

    private void LookMoveDirection()
    {
        if (isMove)
        {
            spot.transform.localScale = new Vector3(
                Mathf.Max(0, spot.transform.localScale.x - 0.05f),
                Mathf.Max(0, spot.transform.localScale.y - 0.05f),
                Mathf.Max(0, spot.transform.localScale.z - 0.05f));


            if (spot.transform.localScale.x <= 0)
            {
                spot.gameObject.SetActive(false);
            }

            Vector3 moveDirection = agent.velocity.normalized;
            if (moveDirection != Vector3.zero) // velocity가 0이 아닐 때만 회전
            {
                moveDirection.y = 0;    // y 값을 0으로 고정하여 수평 방향으로만 회전
                capsule.transform.forward = moveDirection;
            }
        }

        if (agent.velocity.magnitude == 0f)
        {
            isMove = false;
            return;
        }

        if (!isMove)
        {
            spot.gameObject.SetActive(false);
        }

    }
}
